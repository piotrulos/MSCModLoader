using MSCLoader;
using System;
using System.IO;
using UnityEngine;


namespace MSCLoader
{
    public class MP3Lib
    {
        public AudioClip GetAudioClipFromMP3ByteArray(FileStream stream)
        {
            AudioClip l_oAudioClip = null;
            //Stream l_oByteStream = new MemoryStream(in_aMP3Data);
            Mp3Sharp.Mp3Stream l_oMP3Stream = new Mp3Sharp.Mp3Stream(stream);
            
            //Get the converted stream data
            MemoryStream l_oConvertedAudioData = new MemoryStream();
            byte[] l_aBuffer = new byte[4096];
            int l_nBytesReturned = 1;
            int l_nTotalBytesReturned = 0;

            while (l_nBytesReturned > 0)
            {
                l_nBytesReturned = l_oMP3Stream.Read(l_aBuffer, 0, l_aBuffer.Length);
                l_oConvertedAudioData.Write(l_aBuffer, 0, l_nBytesReturned);
                l_nTotalBytesReturned += l_nBytesReturned;              
            }

            ModConsole.Print("MP3 file has " + l_oMP3Stream.ChannelCount + " channels with a frequency of " + l_oMP3Stream.Frequency);

            byte[] l_aConvertedAudioData = l_oConvertedAudioData.ToArray();
            ModConsole.Print("Converted Data has " + l_aConvertedAudioData.Length + " bytes of data");

            //Convert the byte converted byte data into float form in the range of 0.0-1.0
            float[] l_aFloatArray = new float[l_aConvertedAudioData.Length / 2];

            for (int i = 0; i < l_aFloatArray.Length; i++)
            {
                //remember that it is SIGNED Int16, not unsigned
                l_aFloatArray[i] = (BitConverter.ToInt16(l_aConvertedAudioData, i * 2) / 32768.0f);
            }

            l_oAudioClip = AudioClip.Create("MySound", l_aFloatArray.Length /2, 2, l_oMP3Stream.Frequency, false);
            l_oAudioClip.SetData(l_aFloatArray, 0);
            
            stream.Close();
            return l_oAudioClip;
        }
    }
}
