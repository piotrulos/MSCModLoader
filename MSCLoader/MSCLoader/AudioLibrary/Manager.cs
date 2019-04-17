using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

#pragma warning disable CS1591 
namespace AudioLibrary
{
    public class Manager : MonoBehaviour
    {
        private class AudioInstance
        {
            public AudioClip audioClip;

            public AudioFileReader reader;

            public float[] dataToSet;

            public int samplesCount;

            public Stream streamToDisposeOnceDone;

            public int channels => reader.WaveFormat.Channels;

            public int sampleRate => reader.WaveFormat.SampleRate;

            public static implicit operator AudioClip(AudioInstance ai) => ai.audioClip;
        }

        private static readonly string[] supportedFormats;

        private static Dictionary<string, AudioClip> cache;

        private static Queue<AudioInstance> deferredLoadQueue;

        private static Queue<AudioInstance> deferredSetDataQueue;

        private static Queue<AudioInstance> deferredSetFail;

        private static Thread deferredLoaderThread;

        private static GameObject managerInstance;

        private static Dictionary<AudioClip, AudioClipLoadType> audioClipLoadType;

        private static Dictionary<AudioClip, AudioDataLoadState> audioLoadState;

        static Manager()
        {
            cache = new Dictionary<string, AudioClip>();
            deferredLoadQueue = new Queue<AudioInstance>();
            deferredSetDataQueue = new Queue<AudioInstance>();
            deferredSetFail = new Queue<AudioInstance>();
            audioClipLoadType = new Dictionary<AudioClip, AudioClipLoadType>();
            audioLoadState = new Dictionary<AudioClip, AudioDataLoadState>();
            supportedFormats = Enum.GetNames(typeof(AudioFormat));
        }

        public static AudioClip Load(string filePath, bool doStream = false, bool loadInBackground = true, bool useCache = true)
        {
            if (!IsSupportedFormat(filePath))
            {
                Debug.LogError("Could not load AudioClip at path '" + filePath + "' it's extensions marks unsupported format, supported formats are: " + string.Join(", ", Enum.GetNames(typeof(AudioFormat))));
                return null;
            }
            AudioClip audioClip;
            if (useCache && cache.TryGetValue(filePath, out audioClip) && audioClip)
            {
                return audioClip;
            }
            StreamReader streamReader = new StreamReader(filePath);
            audioClip = Load(streamReader.BaseStream, GetAudioFormat(filePath), filePath, doStream, loadInBackground, true);
            if (useCache)
            {
                cache[filePath] = audioClip;
            }
            return audioClip;
        }

        public static AudioClip Load(Stream dataStream, AudioFormat audioFormat, string unityAudioClipName, bool doStream = false, bool loadInBackground = true, bool diposeDataStreamIfNotNeeded = true)
        {
            AudioClip audioClip = null;
            AudioFileReader reader = null;
            try
            {
                reader = new AudioFileReader(dataStream, audioFormat);
                AudioInstance audioInstance = new AudioInstance
                {
                    reader = reader,
                    samplesCount = (int)(reader.Length / (reader.WaveFormat.BitsPerSample / 8))
                };
                if (doStream)
                {
                    audioClip = AudioClip.Create(unityAudioClipName, audioInstance.samplesCount / audioInstance.channels, audioInstance.channels, audioInstance.sampleRate, doStream, delegate (float[] target)
                    {
                        reader.Read(target, 0, target.Length);
                    }, delegate (int target)
                    {
                        if(audioInstance.channels == 1)
                            reader.Seek(target * 4, SeekOrigin.Begin);
                        else
                            reader.Seek(target * 8, SeekOrigin.Begin);
                    });
                    audioInstance.audioClip = audioClip;
                    SetAudioClipLoadType(audioInstance, AudioClipLoadType.Streaming);
                    SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loaded);
                }
                else
                {
                    audioClip = AudioClip.Create(unityAudioClipName, audioInstance.samplesCount / audioInstance.channels, audioInstance.channels, audioInstance.sampleRate, doStream);
                    audioInstance.audioClip = audioClip;
                    if (diposeDataStreamIfNotNeeded)
                    {
                        audioInstance.streamToDisposeOnceDone = dataStream;
                    }
                    SetAudioClipLoadType(audioInstance, AudioClipLoadType.DecompressOnLoad);
                    SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loading);
                    if (loadInBackground)
                    {
                        object obj = deferredLoadQueue;
                        lock (obj)
                        {
                            deferredLoadQueue.Enqueue(audioInstance);
                        }
                        RunDeferredLoaderThread();
                        EnsureInstanceExists();
                    }
                    else
                    {
                        audioInstance.dataToSet = new float[audioInstance.samplesCount];
                        audioInstance.reader.Read(audioInstance.dataToSet, 0, audioInstance.dataToSet.Length);
                        audioInstance.audioClip.SetData(audioInstance.dataToSet, 0);
                        SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loaded);
                    }
                }
            }
            catch (Exception ex)
            {
                //SetAudioClipLoadState(audioClip, AudioDataLoadState.Failed);
                MSCLoader.ModConsole.Error(string.Concat(new object[]
                {
                    unityAudioClipName,
                    " - Failed:",
                    ex.Message
                }));
                Debug.LogError(string.Concat(new object[]
                {
                    "Could not load AudioClip named '",
                    unityAudioClipName,
                    "', exception:",
                    ex
                }));
            }
            return audioClip;
        }

        private static void RunDeferredLoaderThread()
        {
            if (deferredLoaderThread == null || !deferredLoaderThread.IsAlive)
            {
                deferredLoaderThread = new Thread(new ThreadStart(DeferredLoaderMain));
                deferredLoaderThread.IsBackground = true;
                deferredLoaderThread.Start();
            }
        }

        private static void DeferredLoaderMain()
        {
            AudioInstance audioInstance = null;
            bool flag = true;
            long num = 100000L;
            while (flag || num > 0L)
            {
                num -= 1L;
                object obj = deferredLoadQueue;
                lock (obj)
                {
                    flag = (deferredLoadQueue.Count > 0);
                    if (!flag)
                    {
                        continue;
                    }
                    audioInstance = deferredLoadQueue.Dequeue();
                }
                num = 100000L;
                try
                {
                    audioInstance.dataToSet = new float[audioInstance.samplesCount];
                    audioInstance.reader.Read(audioInstance.dataToSet, 0, audioInstance.dataToSet.Length);
                    audioInstance.reader.Close();
                    audioInstance.reader.Dispose();
                    if (audioInstance.streamToDisposeOnceDone != null)
                    {
                        audioInstance.streamToDisposeOnceDone.Close();
                        audioInstance.streamToDisposeOnceDone.Dispose();
                        audioInstance.streamToDisposeOnceDone = null;
                    }
                    object obj2 = deferredSetDataQueue;
                    lock (obj2)
                    {
                        deferredSetDataQueue.Enqueue(audioInstance);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    object obj3 = deferredSetFail;
                    lock (obj3)
                    {
                        deferredSetFail.Enqueue(audioInstance);
                    }
                }
            }
        }

        private void Update()
        {
            AudioInstance audioInstance = null;
            bool flag = true;
            while (flag)
            {
                object obj = deferredSetDataQueue;
                lock (obj)
                {
                    flag = (deferredSetDataQueue.Count > 0);
                    if (!flag)
                    {
                        break;
                    }
                    audioInstance = deferredSetDataQueue.Dequeue();
                }
                audioInstance.audioClip.SetData(audioInstance.dataToSet, 0);
                SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loaded);
                audioInstance.audioClip = null;
                audioInstance.dataToSet = null;
            }
            object obj2 = deferredSetFail;
            lock (obj2)
            {
                while (deferredSetFail.Count > 0)
                {
                    audioInstance = deferredSetFail.Dequeue();
                    SetAudioClipLoadState(audioInstance, AudioDataLoadState.Failed);
                }
            }
        }

        private static void EnsureInstanceExists()
        {
            if (!managerInstance)
            {
                managerInstance = new GameObject("Runtime AudioClip Loader Manger singleton instance");
                managerInstance.hideFlags = HideFlags.HideAndDontSave;
                managerInstance.AddComponent<Manager>();
            }
        }

        public static void SetAudioClipLoadState(AudioClip audioClip, AudioDataLoadState newLoadState)
        {
            audioLoadState[audioClip] = newLoadState;
        }

        public static AudioDataLoadState GetAudioClipLoadState(AudioClip audioClip)
        {
            AudioDataLoadState result = AudioDataLoadState.Failed;
            if (audioClip != null)
            {
                result = audioClip.loadState;
                audioLoadState.TryGetValue(audioClip, out result);
            }
            return result;
        }

        public static void SetAudioClipLoadType(AudioClip audioClip, AudioClipLoadType newLoadType)
        {
            audioClipLoadType[audioClip] = newLoadType;
        }

        public static AudioClipLoadType GetAudioClipLoadType(AudioClip audioClip)
        {
            AudioClipLoadType result = (AudioClipLoadType)(-1);
            if (audioClip != null)
            {
                result = audioClip.loadType;
                audioClipLoadType.TryGetValue(audioClip, out result);
            }
            return result;
        }

        private static string GetExtension(string filePath)
        {
            return Path.GetExtension(filePath).Substring(1).ToLower();
        }

        public static bool IsSupportedFormat(string filePath)
        {
            return supportedFormats.Contains(GetExtension(filePath));
        }

        public static AudioFormat GetAudioFormat(string filePath)
        {
            AudioFormat result = AudioFormat.unknown;
            try
            {
                result = (AudioFormat)Enum.Parse(typeof(AudioFormat), GetExtension(filePath), true);
            }
            catch
            {
            }
            return result;
        }

        public static void ClearCache()
        {
            cache.Clear();
        }
    }
}
#pragma warning restore CS1591