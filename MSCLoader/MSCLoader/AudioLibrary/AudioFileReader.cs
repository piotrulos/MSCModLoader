#if !Mini
using NAudio.Flac;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;

namespace AudioLibrary;


internal enum AudioFormat
{
    wav,
    mp3,
    aiff,
    ogg,
    ogv,
    flac,
    unknown = -1
}
internal class AudioFileReader : WaveStream, ISampleProvider
{
    private WaveStream readerStream;

    private readonly SampleChannel sampleChannel;

    private readonly int destBytesPerSample;

    private readonly int sourceBytesPerSample;

    private readonly long length;

    private readonly object lockObject;

    public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

    public override long Length => length;

    public override long Position
    {
        get => SourceToDest(readerStream.Position);
        set
        {
            object obj = lockObject;
            lock (obj)
            {
                readerStream.Position = DestToSource(value);
            }
        }
    }

    public float Volume
    {
        get => sampleChannel.Volume;
        set => sampleChannel.Volume = value;
    }

    public AudioFileReader(Stream stream, AudioFormat format)
    {
        lockObject = new object();
        CreateReaderStream(stream, format);
        sourceBytesPerSample = (readerStream.WaveFormat.BitsPerSample / 8) * readerStream.WaveFormat.Channels;
        sampleChannel = new SampleChannel(readerStream, false);
        destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
        length = SourceToDest(readerStream.Length);
    }

    private void CreateReaderStream(Stream stream, AudioFormat format)
    {
        switch (format)
        {
            case AudioFormat.wav:
                readerStream = new WaveFileReader(stream);
                if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                    readerStream = new BlockAlignReductionStream(readerStream);
                }
                break;
            case AudioFormat.mp3:
                readerStream = new Mp3FileReader(stream);
                break;
            case AudioFormat.aiff:
                readerStream = new AiffFileReader(stream);
                break;
            case AudioFormat.ogg:
            case AudioFormat.ogv:
                readerStream = new VorbisWaveReader(stream);
                break;
            case AudioFormat.flac:
                readerStream = new FlacReader(stream);
                break;
            default:
                System.Console.WriteLine($"Audio format {format} is not supported");
                break;
        }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        WaveBuffer waveBuffer = new WaveBuffer(buffer);
        int count2 = count / 4;
        int num = Read(waveBuffer.FloatBuffer, offset / 4, count2);
        return num * 4;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        object obj = lockObject;
        int result;
        lock (obj)
        {
            result = sampleChannel.Read(buffer, offset, count);
        }
        return result;
    }

    private long SourceToDest(long sourceBytes)
    {
        return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
    }

    private long DestToSource(long destBytes)
    {
        return sourceBytesPerSample * (destBytes / destBytesPerSample);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && readerStream != null)
        {
            readerStream.Dispose();
            readerStream = null;
        }
        base.Dispose(disposing);
    }
}
#endif