#if !Mini
using AudioLibrary.MP3_Streaming;
using System;
using static AudioLibrary.MP3_Streaming.MP3Stream;

namespace MSCLoader;

/// <summary>
/// Audio library (Play online "audio/mpeg" streams)
/// </summary>
public class ModAudioStream : MonoBehaviour
{
    /// <summary>
    /// Your AudioSource goes here
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Song info from metadata (if available)
    /// </summary>
    public string songInfo;

    /// <summary>
    /// Stream title (if available)
    /// </summary>
    public string streamTitle;

    /// <summary>
    /// Bitrate info (if available)
    /// </summary>
    public string bitrateInfo;

    /// <summary>
    /// Show debug info
    /// </summary>
    public bool showDebug = false;

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
        if (streamURL.StartsWith("https://"))
        {
            ModConsole.Warning("ModAudioStream - https:// urls are not supported, please rename your stream url to http://");
            return;
        }
        if (audioSource == null)
            audioSource = gameObject.GetComponent<AudioSource>();
        mp3s.audioSource = audioSource;
        mp3s.PlayStream(streamURL);
        if (showDebug)
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
        if (mp3s.error)
        {
            ModConsole.Error(mp3s.error_info);
            StopStream();
            return;
        }
        mp3s.UpdateLoop();
        bufferInfo = mp3s.buffer_info;
        songInfo = mp3s.song_info;
        streamTitle = mp3s.stream_title;
        bitrateInfo = mp3s.bitrate_info;
    }

    void OnGUI()
    {
        if (showDebugInfo)
        {
            string playbackColor = mp3s.playbackState switch
            {
                StreamingPlaybackState.Stopped => "red",
                StreamingPlaybackState.Paused => "yellow",
                StreamingPlaybackState.Buffering => "orange",
                StreamingPlaybackState.Playing => "lime",
                _ => "orange",
            };
            string text = $"<color={playbackColor}>{mp3s.playbackState}</color> | Buffer: <color=orange>{bufferInfo}</color> | Metadata: <color=orange>{songInfo}</color>";
            if (mp3s.IsBufferNearlyFull)
            {
                text += " | <color=red>Buffer full</color>";
            }
            text += $"{Environment.NewLine}Encoding: {(mp3s.encoding_info == "audio/mpeg" ? "<color=lime>" : "<color=red>")}{mp3s.encoding_info}</color> | Bitrate: <color=orange>{mp3s.bitrate_info}</color> | Stream: <color=orange>{mp3s.stream_title}</color>";
            GUI.Label(new Rect(1, Screen.height - 38, Screen.width, 43), text);
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

}

#endif