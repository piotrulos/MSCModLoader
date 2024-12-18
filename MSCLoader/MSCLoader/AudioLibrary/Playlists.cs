#if !Mini
using System.Collections.Generic;
using System.IO;

namespace MSCLoader;

/// <summary>
/// Parse playlists files
/// </summary>
public class Playlists
{
    /// <summary>
    /// Parse *.pls playlist file and return List of paths
    /// </summary>
    /// <param name="path">Path to *.pls file</param>
    /// <returns>List of paths to files/urls</returns>
    public static List<string> plsPlaylist(string path)
    {
        FileStream s1 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return plsPlaylist(s1);
    }

    /// <summary>
    /// Parse *.pls playlist file and return List of paths
    /// </summary>
    /// <param name="stream">File stream</param>
    /// <returns>List of paths to files/urls</returns>
    public static List<string> plsPlaylist(Stream stream)
    {
        List<string> playlist = new List<string>();
        StreamReader streamReader = new StreamReader(stream);
        if (!streamReader.EndOfStream)
        {
            string header = streamReader.ReadLine().Trim();
            if (header.Trim() != "[playlist]")
            {
                return playlist;
            }
        }
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine().Trim();
            if (line.StartsWith("File"))
            {
                string path = null;
                try
                {
                    path = line.Substring(line.IndexOf('=') + 1);
                }
                catch { }

                if (path != null)
                    playlist.Add(path);
            }
            else if (line.StartsWith("Title"))
            {
                //skip
            }
            else if (line.StartsWith("Length"))
            {
                //skip
            }
        }
        return playlist;
    }

    /// <summary>
    /// Parse *.m3u and *.m3u8 playlist files and return List of paths
    /// </summary>
    /// <param name="path">Path to .m3u or *.m3u8 file</param>
    /// <returns>List of paths to files/urls</returns>
    public static List<string> m3uPlaylist(string path)
    {
        FileStream s1 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return m3uPlaylist(s1);
    }

    /// <summary>
    /// Parse *.m3u and *.m3u8 playlist files and return List of paths
    /// </summary>
    /// <param name="stream">File stream</param>
    /// <returns>List of paths to files/urls</returns>
    public static List<string> m3uPlaylist(Stream stream)
    {
        List<string> playlist = new List<string>();
        StreamReader streamReader = new StreamReader(stream);
        bool IsExtended = false;
        if (!streamReader.EndOfStream)
        {
            string header = streamReader.ReadLine().Trim();
            if (header == "#EXTM3U")
            {
                IsExtended = true;
            }
            else
            {
                IsExtended = false;
                playlist.Add(header);
            }
        }
        bool prevLineIsExtInf = false;
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            if (line.StartsWith("#"))
            {
                if (IsExtended)
                {
                    if (line.StartsWith("#EXTINF"))
                    {
                        prevLineIsExtInf = true;
                        //skip
                    }
                    else if (line.StartsWith("#EXTALB"))
                    {
                        //skip

                    }
                    else if (line.StartsWith("#EXTART"))
                    {
                        //skip
                    }
                }
            }
            else
            {
                if (!IsExtended || !prevLineIsExtInf)
                {
                    //skip
                }
                playlist.Add(line);
                prevLineIsExtInf = false;
            }
        }
        return playlist;
    }
}
#endif