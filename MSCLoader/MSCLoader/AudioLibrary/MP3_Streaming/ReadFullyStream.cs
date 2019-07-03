using System;
using System.IO;
using System.Text;

namespace AudioLibrary.MP3_Streaming
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public delegate void StreamTitleChangedEventHandler(object sender, StreamTitleChangedEventArgs e);
    public class StreamTitleChangedEventArgs : EventArgs
    {
        public readonly string Title;

        public StreamTitleChangedEventArgs(string title)
        {
            Title = title;
        }
    }
    public class ReadFullyStream : Stream
    {
        public int MetaInt;
        private string StreamTitle;

        private int receivedBytes;
        private Stream netStream;
        private bool connected = false;

        private int read;
        private int leftToRead;
        private int thisOffset;
        private int bytesRead;
        private int bytesLeftToMeta;
        private int metaLen;
        private byte[] metaInfo;

        private long pos;

        // <summary>
        // Is fired, when a new StreamTitle is received
        // </summary>
        public event StreamTitleChangedEventHandler StreamTitleChanged;

        public ReadFullyStream(Stream respStream)
        {
            StreamTitle = string.Empty;
            netStream = respStream;
            connected = true;
        }

        // <summary>
        // Parses the received Meta Info
        // </summary>
        // <param name="metaInfo"></param>
        private void ParseMetaInfo(byte[] metaInfo)
        {
            string metaString = Encoding.ASCII.GetString(metaInfo);
            string newStreamTitle = metaString.Split(new string[] { "';" }, StringSplitOptions.None)[0].Replace("StreamTitle='", string.Empty);
            if (!newStreamTitle.Equals(StreamTitle))
            {
               StreamTitle = newStreamTitle;
               OnStreamTitleChanged();
            }
        }

        // <summary>
        // Fires the StreamTitleChanged event
        // </summary>
        protected virtual void OnStreamTitleChanged()
        {
            StreamTitleChanged?.Invoke(this, new StreamTitleChangedEventArgs(StreamTitle));
        }

        // <summary>
        // Gets a value that indicates whether the ShoutcastStream supports reading.
        // </summary>
        public override bool CanRead => connected;

        // <summary>
        // Gets a value that indicates whether the ShoutcastStream supports seeking.
        // This property will always be false.
        // </summary>
        public override bool CanSeek => false;

        // <summary>
        // Gets a value that indicates whether the ShoutcastStream supports writing.
        // This property will always be false.
        // </summary>
        public override bool CanWrite => false;


        // <summary>
        // Flushes data from the stream.
        // This method is currently not supported
        // </summary>
        public override void Flush()
        {
            return;
        }

        // <summary>
        // Gets the length of the data available on the Stream.
        // This property is not currently supported and always thows a <see cref="NotSupportedException"/>.
        // </summary>
        public override long Length => pos;

        // <summary>
        // Gets or sets the current position in the stream.
        // This property is not currently supported and always thows a <see cref="NotSupportedException"/>.
        // </summary>
        public override long Position
        {
            get => pos;
            set => throw new NotSupportedException();
        }

        // <summary>
        // Reads data from the ShoutcastStream.
        // </summary>
        // <param name="buffer">An array of bytes to store the received data from the ShoutcastStream.</param>
        // <param name="offset">The location in the buffer to begin storing the data to.</param>
        // <param name="count">The number of bytes to read from the ShoutcastStream.</param>
        // <returns>The number of bytes read from the ShoutcastStream.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                read = 0;
                leftToRead = count;
                thisOffset = offset;
                bytesRead = 0;
                bytesLeftToMeta = ((MetaInt - receivedBytes) > count) ? count : (MetaInt - receivedBytes);

                while (bytesLeftToMeta > 0 && (read = netStream.Read(buffer, thisOffset, bytesLeftToMeta)) > 0)
                {
                    leftToRead -= read;
                    thisOffset += read;
                    bytesRead += read;
                    receivedBytes += read;
                    bytesLeftToMeta -= read;
                }

                // read metadata
                if (receivedBytes == MetaInt)
                {
                    metaLen = netStream.ReadByte();
                    if (metaLen > 0)
                    {
                        metaInfo = new byte[metaLen * 16];
                        int len = 0;
                        while ((len += netStream.Read(metaInfo, len, metaInfo.Length - len)) < metaInfo.Length) ;
                        ParseMetaInfo(metaInfo);
                    }
                    receivedBytes = 0;
                }

                while (leftToRead > 0 && (read = netStream.Read(buffer, thisOffset, leftToRead)) > 0)
                {
                    leftToRead -= read;
                    thisOffset += read;
                    bytesRead += read;
                    receivedBytes += read;
                }

                pos += bytesRead;
                return bytesRead;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // <summary>
        // Closes the ShoutcastStream.
        // </summary>
        public override void Close()
        {
            connected = false;
            netStream.Close();
        }

        // <summary>
        // Sets the current position of the stream to the given value.
        // This Method is not currently supported and always throws a <see cref="NotSupportedException"/>.
        // </summary>
        // <param name="offset"></param>
        // <param name="origin"></param>
        // <returns></returns>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        // <summary>
        // Sets the length of the stream.
        // This Method always throws a <see cref="NotSupportedException"/>.
        // </summary>
        // <param name="value"></param>
        public override void SetLength(long value) => throw new NotSupportedException();

        // <summary>
        // Writes data to the ShoutcastStream.
        // This method is not currently supported and always throws a <see cref="NotSupportedException"/>.
        // </summary>
        // <param name="buffer"></param>
        // <param name="offset"></param>
        // <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
