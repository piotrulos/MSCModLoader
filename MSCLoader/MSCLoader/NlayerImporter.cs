using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NLayer;
using System.IO;
using MSCLoader;
using System;

public class NlayerImporter : MonoBehaviour
{

    public bool importStreaming = true;

    /// <summary>
    /// Total number of pieces that have to be imported.
    /// </summary>
    private int totalImportCount;

    /// <summary>
    /// The index of the current piece that has to be imported.
    /// </summary>
    private int currentImportIndex;

    /// <summary>
    /// The size of the import buffer.
    /// </summary>
    private int importBufferSize = 4410;

    /// <summary>
    /// MPEG decoder.
    /// </summary>
    private MpegFile mpegFile;

    private AudioClip audioClip;

    /// <summary>
    /// Imports an mp3 file. Only the start of a file is imported at first.
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
        ModConsole.Print("start");
        try
        {
            if (audioClip != null)
                AudioClip.Destroy(audioClip);

            totalImportCount = 0;

            if (mpegFile != null)
                mpegFile.Dispose();

            mpegFile = new MpegFile(Path.GetFullPath(filePath));
            int lengthSamples = (int)(mpegFile.Length / mpegFile.Channels) / sizeof(float);

            audioClip = AudioClip.Create(Path.GetFileName(filePath), lengthSamples, mpegFile.Channels, mpegFile.SampleRate, false);

            totalImportCount = (lengthSamples * mpegFile.Channels) / importBufferSize;

            currentImportIndex = 0;

            int min = importStreaming ? Mathf.Min(totalImportCount, 345) : totalImportCount;

            for (int i = 0; i < min; i++)
            {
                float[] buffer = new float[(importBufferSize)];
                mpegFile.ReadSamples(buffer, 0, (importBufferSize));
                audioClip.SetData(buffer, currentImportIndex * (importBufferSize / 2));
                currentImportIndex++;
            }
            ModConsole.Print("stop");

        }
        catch(Exception e)
        {
            ModConsole.Error(e.Message + e.InnerException + e.StackTrace);
        }

        return audioClip;
    }

    void Update()
    {
        //Import more and more of the song
        if (currentImportIndex < totalImportCount - 1)
        {
            float[] buffer = new float[(importBufferSize)];
            mpegFile.ReadSamples(buffer, 0, (importBufferSize));
            audioClip.SetData(buffer, currentImportIndex * (importBufferSize / 2));
            currentImportIndex++;
        }
    }
}