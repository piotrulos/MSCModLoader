//uAudio library, found on github.

#pragma warning disable 0649
using UnityEngine;
using System;
using uAudio.uAudio_backend;
using MSCLoader;

namespace uAudio
{
#pragma warning disable CS1591
    public class uAudioPlayer : MonoBehaviour, IAudioPlayer
    {
        #region var
        //public Action SongFinish;
        uAudio_backend.uAudio _uAudio;


        public AudioSource myAudioSource;

        //public System.Action PlayBackStopped { get { return _PlaybackStopped; } set { _PlaybackStopped = value; } }
        public Action<float> Update_UI_songTime;
        public string targetFile;
        Action<PlayBackState> _sendPlaybackState;
        public Action<PlayBackState> sendPlaybackState
        {
            get
            {
                return _sendPlaybackState;
            }

            set
            {
                _sendPlaybackState = value;
            }
        }
        public int SongLength { get { return _uAudio.SongLength; } }

        //System.Action _PlaybackStopped;
        bool updateTime = false;
        PlayBackState State;
        public bool SongDone = false;
        bool flare_SongEnd = false;
        float[] _getAudioData_sampler;
        public NLayer.MpegFile playbackDevice;

        uAudioDemo.Mp3StreamingDemo.ReadFullyStream readFullyStream;

        //  [Range(0f,1.0f)]
        public float start_volume_Offset = 0.5f;

        public float Volume_Offset
        {
            get
            {
                return start_volume_Offset;
            }

            set
            {
                if (_uAudio != null)
                    _uAudio.Volume = value;
                start_volume_Offset = value;
            }
        }
        public uAudio_backend.uAudio UAudio
        {
            get
            {
                if (_uAudio == null)
                {
                    _uAudio = new uAudio_backend.uAudio();
                    _uAudio.SetAudioFile(targetFile);
                    _uAudio.Volume = start_volume_Offset;
                    _uAudio.sendPlaybackState = (PlayBackState c) => {
                        _sendPlaybackState(c);
                    };
                }
                return _uAudio;
            }
        }

        public bool IsPlaying => State == PlayBackState.Playing;

        Action IAudioPlayer.SLEEP
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        void uAudio_stopped()
        {
            updateTime = false;
        }

        public string AudioTitle
        {
            get
            {
                if (_uAudio != null)
                    return _uAudio.AudioTitle;
                else
                    return string.Empty;
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                if (_uAudio != null && State != PlayBackState.Stopped)
                    return _uAudio.CurrentTime;// - new TimeSpan(0, 0, 1);
                else
                    return endSongTime;
            }
            set
            {
                if (_uAudio != null)
                    _uAudio.CurrentTime = value;
            }
        }


        public float Pan
        {
            get
            {
                if (myAudioSource != null)
                    return myAudioSource.panStereo;
                else
                    return 0;
            }
            set
            {
                if (myAudioSource != null)
                    myAudioSource.panStereo = value;
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                if (_uAudio != null)
                    return _uAudio.TotalTime;
                else
                    return TimeSpan.Zero;
            }
        }

        public float Volume
        {
            get => myAudioSource.volume;
            set => myAudioSource.volume = value;
        }

        public float Volume_BackEnd
        {
            get
            {
                if (_uAudio != null)
                    return _uAudio.Volume;
                else
                    return Volume_Offset;
            }
            set
            {
                if (_uAudio != null)
                    _uAudio.Volume = value;
                Volume_Offset = value;
            }
        }

        public PlayBackState PlaybackState => State;

        public void ChangeCurrentVolume(float volumeIN)
        {
            Volume = volumeIN;
        }

        string IAudioPlayer.current_TargetFile_Loaded => targetFile;

        public void ChangeCurrentTime(TimeSpan timeIN)
        {
            CurrentTime = timeIN;
        }
        #endregion ---vars---

        #region funcs
        void Update()
        {
            if (updateTime)
            {
                if (State == PlayBackState.Playing)
                {
                    float newValue = (float)CurrentTime.TotalSeconds;

                    if (Update_UI_songTime != null)
                        Update_UI_songTime(newValue);
                }
            }

            if (flare_SongEnd)
            {
                flare_SongEnd = false;
                SongEnd();
                flare_SongEnd = false;
            }
        }

        bool _loadedTarget = false;
        public void LoadFile(string targetFileIN)
        {
            targetFile = targetFileIN;
            if (!_loadedTarget || UAudio.targetFile != targetFile)
            {
                _loadedTarget = true;
                UAudio.LoadFile(targetFileIN);
            }
        }

        public void SetFile(string targetFileIN)
        {
            targetFile = targetFileIN;
        }
        public float SetFilse(string targetFileIN)
        {
            //targetFile = targetFileIN;
            float time =0;
            UAudio.targetFile = targetFileIN;
            if (UAudio.LoadMainOutputStream())
            {
                time = (float)TotalTime.TotalSeconds;
            }
            return time;
        }
        TimeSpan endSongTime = TimeSpan.Zero;
        void SongEnd()
        {
            try
            {
                endSongTime = CurrentTime;
                if (readFullyStream != null)
                {
                    readFullyStream.Close();
                }

                myAudioSource.Stop();
                _uAudio.Stop();

                myAudioSource.clip = null;
                //    myAudioSource.time = 0;
                updateTime = false;
                _loadedTarget = false;
                State = PlayBackState.Stopped;
                try
                {
                    if (sendPlaybackState != null)
                        sendPlaybackState(PlayBackState.Stopped);
                }
                catch
                {
                   ModConsole.Error("sendPlaybackState #897j8h2432a1q");
                }
            }
            catch
            {
                throw new Exception("Song end #7cgf87dcf7sd8csd");
            }
        }

        public void Play(TimeSpan? startOff = null)
        {
            Stop();
            if (State != PlayBackState.Playing)
            {
                if (State == PlayBackState.Paused)
                    Pause();
                else
                {
                    State = PlayBackState.Playing;

                    try
                    {
                        // LoadFile(targetFile);
                        SongDone = false;
                        flare_SongEnd = false;
                        UAudio.targetFile = targetFile;

                        if (myAudioSource.clip == null)
                        {
                            if (UAudio.LoadMainOutputStream())
                            {
                                long song_sampleSize;

                                AudioClip.PCMReaderCallback SongReadLoop;
                                SongReadLoop = new AudioClip.PCMReaderCallback(Song_Stream_Loop);

                                System.IO.Stream s = System.IO.File.OpenRead(targetFile);

                                if (readFullyStream != null)
                                    readFullyStream.Dispose();

                                readFullyStream = new uAudioDemo.Mp3StreamingDemo.ReadFullyStream(s);
                                readFullyStream.stream_CanSeek = true;

                                byte[] buff = new byte[1024];
                                NLayer.MpegFile c = new NLayer.MpegFile(readFullyStream, true);
                                //playbackDevice = m;

                                if (startOff == TimeSpan.Zero)
                                    c.ReadSamples(buff, 0, buff.Length);

                                playbackDevice = c;
                                //   UAudio.TotalTime
                                //     song_sampleSize = playbackDevice.Length;// * playbackDevice.SampleRate;
                                song_sampleSize = UAudio.SongLength;
                                // hack
                                // song_sampleSize = int.MaxValue;// need to better alocate this

                                int setSongSize;
                                if (song_sampleSize > int.MaxValue)
                                {
                                    ModConsole.Error("AudioPlayer - Song size over size on int");
                                    setSongSize = int.MaxValue;
                                }
                                else
                                    setSongSize = (int)song_sampleSize;

                                myAudioSource.clip =
                              AudioClip.Create("uAudio_song", setSongSize,
                                        c.WaveFormat.Channels,
                                        c.WaveFormat.SampleRate,
                                        true, SongReadLoop);

                                if (!startOff.HasValue)
                                    CurrentTime = TimeSpan.Zero;
                                else
                                    CurrentTime = startOff.Value;



                                try
                                {
                                    if (sendPlaybackState != null)
                                        sendPlaybackState(PlayBackState.Playing);
                                }
                                catch
                                {
                                    ModConsole.Error("theAudioStream_sendStartLoopPump");
                                }
                            }
                            else
                                myAudioSource.clip = null;
                        }
                        else
                        {

                        }

                        if (myAudioSource.clip != null)
                        {
                            if (!myAudioSource.isPlaying)
                                myAudioSource.PlayDelayed(1f);
                            // updateTime = true;
                        }
                        else
                            State = PlayBackState.Stopped;
                    }
                    catch (Exception ex)
                    {
                        State = PlayBackState.Stopped;
                        ModConsole.Error(ex.ToString());
                        Debug.Log(ex);
                    }
                }
            }
        }

        void Song_Stream_Loop(float[] data)
        {
            try
            {
                if (!SongDone)
                {
                    //if (_getAudioData_sampler == null)
                    //    _getAudioData_sampler = new float[data.Length];

                    //int got = playbackDevice.Read(data, 0, data.Length);
                    int got = _uAudio.uwa.audioPlayback.inputStream.Read(data, 0, data.Length);
                    //int got = _uAudio.uwa.audioPlayback.inputStream.Read(_getAudioData_sampler, 0, _getAudioData_sampler.Length);

                    if (got <= 0)
                    {
                        SongDone = true;
                    }
                    //    for (int i = got - 1; i < _getAudioData_sampler.Length; i++)
                    //        _getAudioData_sampler[i] = 0;
                    //}

                    //Array.Copy(_getAudioData_sampler, data, data.Length);
                }
                else
                {
                    flare_SongEnd = true;
                }
            }
            catch
            {
                Debug.LogError("Decode Error #8f76s8dsvsd");
            }
        }

        public void Pause()
        {
            if (State == PlayBackState.Playing)
            {
                myAudioSource.Pause();
                State = PlayBackState.Paused;
                try
                {
                    if (sendPlaybackState != null)
                        sendPlaybackState(PlayBackState.Paused);
                }
                catch
                {
                   ModConsole.Error("sendPlaybackState #whrth546h56h56");
                }
            }
            else
            {
                if (State == PlayBackState.Paused)
                {
                    myAudioSource.UnPause();
                    State = PlayBackState.Playing; try
                    {
                        if (sendPlaybackState != null)
                            sendPlaybackState(PlayBackState.Playing);
                    }
                    catch
                    {
                       ModConsole.Error("sendPlaybackState #56y53y5tge56");
                    }
                }
            }
        }

        public void Stop()
        {
            if (State != PlayBackState.Stopped)
            {
                SongEnd();
            }
        }
        #endregion ---funcs---

        #region Dispose
        void OnApplicationQuit()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (readFullyStream != null)
            {
                readFullyStream.Close();
            }
            if (_uAudio != null)
            {
                _uAudio.Dispose();
                _uAudio = null;
            }
            _loadedTarget = false;
        }
        #endregion ---Dispose---
    }
}
#pragma warning restore CS1591
