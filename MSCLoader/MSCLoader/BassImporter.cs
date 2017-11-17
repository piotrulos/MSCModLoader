using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Un4seen.Bass;
using System;


public class BassImporter 
{
    //Browser browser;

    private int sample;
    private int channel;

    /// <summary>
    /// Imports an mp3 file. Only the start of a file is actually imported.
    /// The remaining part of the file will be imported bit by bit to speed things up. 
    /// </summary>
    /// <returns>
    /// Audioclip containing the song.
    /// </returns>
    /// <param name='filePath'>
    /// Path to mp3 file.
    /// </param>
    public AudioClip ImportFile(string filePath)
    {
        //get license from http://bass.radio42.com/bass_purchase.html
        BassNet.Registration ("smieci.kosmo@gmail.com", "2X28222422152222");

        AudioClip audioClip = new AudioClip();

        if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
        {
            sample = Bass.BASS_SampleLoad(filePath, 0, 0, 1, BASSFlag.BASS_SAMPLE_FLOAT);

            BASS_SAMPLE info = Bass.BASS_SampleGetInfo(sample);

            int lengthSamples = (int)(info.length / sizeof(float));

            audioClip = AudioClip.Create(Path.GetFileNameWithoutExtension(filePath), lengthSamples / info.chans, info.chans, info.freq, false);
            float[] data = new float[lengthSamples];
            Bass.BASS_SampleGetData(sample, data);

            audioClip.SetData(data, 0);

            // free the Sample
            Bass.BASS_SampleFree(sample);
            // free BASS
            Bass.BASS_Free();
        }
        return audioClip;
    }

}
