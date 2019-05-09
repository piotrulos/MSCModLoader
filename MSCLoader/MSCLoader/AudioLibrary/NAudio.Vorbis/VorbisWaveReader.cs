using System;
using NAudio.Wave;
using NVorbis;
using System.IO;

namespace NAudio.Vorbis
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class VorbisWaveReader : WaveStream, IDisposable, ISampleProvider, IWaveProvider
    {
        private VorbisReader _reader;
        private WaveFormat _waveFormat;

        public VorbisWaveReader(string fileName)
        {
            _reader = new VorbisReader(fileName);

            _waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_reader.SampleRate, _reader.Channels);
        }

        public VorbisWaveReader(Stream sourceStream)
        {
            _reader = new VorbisReader(sourceStream, false);

            _waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_reader.SampleRate, _reader.Channels);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            base.Dispose(disposing);
        }

        public override WaveFormat WaveFormat => _waveFormat;


        public override long Length => (long)(_reader.TotalTime.TotalSeconds * _waveFormat.SampleRate * _waveFormat.Channels * sizeof(float));

        public override long Position
        {
            get => (long)(_reader.DecodedTime.TotalSeconds * _reader.SampleRate * _reader.Channels * sizeof(float));
            set
            {
                if (value < 0 || value > Length) throw new ArgumentOutOfRangeException("value");

                _reader.DecodedTime = TimeSpan.FromSeconds((double)value / _reader.SampleRate / _reader.Channels / sizeof(float));
            }
        }

        // This buffer can be static because it can only be used by 1 instance per thread
        [ThreadStatic]
        static float[] _conversionBuffer = null;

        public override int Read(byte[] buffer, int offset, int count)
        {
            // adjust count so it is in floats instead of bytes
            count /= sizeof(float);

            // make sure we don't have an odd count
            count -= count % _reader.Channels;

            // get the buffer, creating a new one if none exists or the existing one is too small
            var cb = _conversionBuffer ?? (_conversionBuffer = new float[count]);
            if (cb.Length < count)
            {
                cb = (_conversionBuffer = new float[count]);
            }

            // let Read(float[], int, int) do the actual reading; adjust count back to bytes
            int cnt = Read(cb, 0, count) * sizeof(float);

            // move the data back to the request buffer
            Buffer.BlockCopy(cb, 0, buffer, offset, cnt);

            // done!
            return cnt;
        }

        public int Read(float[] buffer, int offset, int count) => _reader.ReadSamples(buffer, offset, count);

        public bool IsParameterChange => _reader.IsParameterChange;

        public void ClearParameterChange() => _reader.ClearParameterChange();

        public int StreamCount => _reader.StreamCount;

        public int? NextStreamIndex { get; set; }

        public bool GetNextStreamIndex()
        {
            if (!NextStreamIndex.HasValue)
            {
                var idx = _reader.StreamCount;
                if (_reader.FindNextStream())
                {
                    NextStreamIndex = idx;
                    return true;
                }
            }
            return false;
        }

        public int CurrentStream
        {
            get => _reader.StreamIndex;
            set
            {
                if (!_reader.SwitchStreams(value))
                {
                    throw new InvalidDataException("The selected stream is not a valid Vorbis stream!");
                }

                if (NextStreamIndex.HasValue && value == NextStreamIndex.Value)
                {
                    NextStreamIndex = null;
                }
            }
        }

        // <summary>
        // Gets the encoder's upper bitrate of the current selected Vorbis stream
        // </summary>
        public int UpperBitrate => _reader.UpperBitrate;

        // <summary>
        // Gets the encoder's nominal bitrate of the current selected Vorbis stream
        // </summary>
        public int NominalBitrate => _reader.NominalBitrate;

        // <summary>
        // Gets the encoder's lower bitrate of the current selected Vorbis stream
        // </summary>
        public int LowerBitrate => _reader.LowerBitrate;

        // <summary>
        // Gets the encoder's vendor string for the current selected Vorbis stream
        // </summary>
        public string Vendor => _reader.Vendor;

        // <summary>
        // Gets the comments in the current selected Vorbis stream
        // </summary>
        public string[] Comments => _reader.Comments;

        // <summary>
        // Gets the number of bits read that are related to framing and transport alone
        // </summary>
        public long ContainerOverheadBits => _reader.ContainerOverheadBits;

        // <summary>
        // Gets stats from each decoder stream available
        // </summary>
        public IVorbisStreamStatus[] Stats => _reader.Stats;
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}