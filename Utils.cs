#pragma warning disable SYSLIB0014

using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace ip;

public static class Utils
{
    public static readonly WebClient WebClient = new();
    public static string? GetHostName(string address)
    {
        try
        {
            IPHostEntry entry = Dns.GetHostEntry(address);
            return entry.HostName;
        }
        catch (SocketException ex)
        {
            //unknown host or
            //not every IP has a name
            //log exception (manage it)
        }

        return null;
    }

    public static string? GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString();

        return null;
    }

    public static bool HasNetwork() => NetworkInterface.GetIsNetworkAvailable();

    public static bool IsInternetAvailable()
    {
        if (!HasNetwork()) return false;

        try
        {
            using (WebClient.OpenRead("http://clients3.google.com/generate_204"))
                return true;
        }
        catch
        {
            return false;
        }
    }

    public static string? GetPublicIPAddress()
    {
        if (!IsInternetAvailable()) return null;

        return WebClient.DownloadString("http://ipinfo.io/ip");
    }
    
    public static IPInfo? GetPublicIPInfo(string? ip = null)
    {
        if (!IsInternetAvailable()) return null;

        string response = WebClient.DownloadString("http://ipinfo.io/" + (ip ?? ""));
        return JsonConvert.DeserializeObject<IPInfo>(response);
    }
}

public class IPInfo
{
    public string ip { get; set; }
    public string hostname { get; set; }
    public string city { get; set; }
    public string region { get; set; }
    public string country { get; set; }
    public string loc { get; set; }
    public string org { get; set; }
    public string postal { get; set; }
    public string timezone { get; set; }
    public string readme { get; set; }
    
    public string CountryFriendly => new RegionInfo(this.country).EnglishName;
}