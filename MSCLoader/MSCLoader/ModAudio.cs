using RuntimeAudioClipLoader;
using System;
using System.IO;
using UnityEngine;

namespace MSCLoader
{
    public class ModAudio : MonoBehaviour
    {
        public AudioSource audioSource;

        public void LoadAudioFromFile(string path, bool doStream, bool background)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Stream stream = new MemoryStream(bytes);
            AudioFormat format = Manager.GetAudioFormat(path);
            string filename = Path.GetFileName(path);

            if (format == AudioFormat.unknown) format = AudioFormat.mp3;

            try
            {
                if (audioSource == null)
                {
                    audioSource = gameObject.GetComponent<AudioSource>();
                }

                audioSource.clip = Manager.Load(stream, format, filename, doStream, background, true);
            }
            catch (Exception e)
            {
                ModConsole.Error(e.Message);
                Debug.Log(e);
                audioSource.clip = null;
            }

        }
        public TimeSpan Time()
        {
            if (audioSource.clip != null)
                return TimeSpan.FromSeconds(audioSource.time);
            else
                return TimeSpan.FromSeconds(0);
        }
        public void Play()
        {
            audioSource.Play();
        }

        public void Stop()
        {
            if(audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
