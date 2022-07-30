using AudioLibrary.MP3_Streaming;
using UnityEngine;

namespace MSCLoader
{
    /// <summary>
    /// Audio library (Play online mp3 streams)
    /// </summary>
    public class ModAudioStream : MonoBehaviour
    {
#if !Mini
        /// <summary>
        /// Your AudioSource goes here
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Song info readed from metadata (if available)
        /// </summary>
        public string songInfo;

        /// <summary>
        /// Show debug info
        /// </summary>
        public bool showDebug =false;

        private bool showDebugInfo;
        private MP3Stream mp3s = new MP3Stream();
        private bool done = false;
        private string bufferInfo;


        /// <summary>
        /// Plays the stream
        /// </summary>
        /// <param name="streamURL">stream url</param>
        public void PlayStream(string streamURL)
        {
            if(audioSource == null)
                audioSource = gameObject.GetComponent<AudioSource>();
            mp3s.audioSource = audioSource;
            mp3s.PlayStream(streamURL);
            if(showDebug)
                showDebugInfo = true;
        }
        /// <summary>
        /// Stops the stream
        /// </summary>
        public void StopStream()
        {
            mp3s.Dispose();
            done = false;
            audioSource.clip = null;
        }
        void FixedUpdate()
        {
            mp3s.UpdateLoop();
            bufferInfo = mp3s.buffer_info;
            songInfo = mp3s.song_info;
        }

        void OnGUI()
        {
            if (showDebugInfo)
            {
                string text = $"<color=orange>{mp3s.playbackState.ToString()}</color> | Buffer: <color=orange>{bufferInfo}</color> | Metadata: <color=orange>{songInfo}</color>";

                if (mp3s.IsBufferNearlyFull)
                {
                    text = $"<color=orange>{mp3s.playbackState.ToString()}</color> | Buffer: <color=orange>{bufferInfo}</color> | Metadata: <color=orange>{songInfo}</color> | <color=red>Buffer full</color>";
                }
                GUI.Label(new Rect(1, Screen.height - 22, Screen.width, 22), text);
                showDebugInfo = showDebug;
            }
        }
        void Update()
        {
            if (mp3s.decomp && !done)
            {
                audioSource.clip = AudioClip.Create("mp3_Stream", int.MaxValue,
                    mp3s.bufferedWaveProvider.WaveFormat.Channels,
                    mp3s.bufferedWaveProvider.WaveFormat.SampleRate,
                    true, new AudioClip.PCMReaderCallback(mp3s.ReadData));

                done = true; //Do not create shitload of audioclips
            }
        }
        void OnApplicationQuit()
        {
            mp3s.Dispose();
        }
        #endif
    }
}