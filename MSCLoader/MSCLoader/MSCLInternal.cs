using System.Net;
using System.Text;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MSCLoader;

internal class MSCLInternal
{
    internal static bool ValidateVersion(string version)
    {
        try
        {
            new Version(version);
        }
        catch
        {
            ModConsole.Error($"Invalid version: {version}{Environment.NewLine}Please use proper version format: (0.0 or 0.0.0 or 0.0.0.0)");
            return false;
        }
        return true;
    }
    internal static string MSCLDataRequest(string reqPath, Dictionary<string, string> data)
    {
        System.Collections.Specialized.NameValueCollection msclData = new System.Collections.Specialized.NameValueCollection { { "msclData", JsonConvert.SerializeObject(data) } };
        string response = "";
        WebClient MSCLDconn = new WebClient();
        MSCLDconn.Headers.Add("user-agent", $"MSCLoader/{ModLoader.MSCLoader_Ver} ({ModLoader.SystemInfoFix()})");
        try
        {
            byte[] sas = MSCLDconn.UploadValues($"{ModLoader.serverURL}/{reqPath}", "POST", msclData);
            response = Encoding.UTF8.GetString(sas, 0, sas.Length);
        }
        catch (Exception e)
        {
            ModConsole.Error($"Request failed with error: {e.Message}");
            Console.WriteLine(e);
            response = "error";
        }
        ModConsole.Warning(response); //TODO: debug remove this
        return response;
    }
}

