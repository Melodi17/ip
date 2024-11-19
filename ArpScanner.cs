using Spectre.Console;

namespace ip;

using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using System.Text.RegularExpressions;

// Borrowed from https://github.com/giuliocomi/arp-scanner
public class ArpScanner
{
    [DllImport("iphlpapi.dll", ExactSpelling = true)]
    private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

    private static uint macAddrLen = (uint)new byte[6].Length;

    public static Dictionary<string, string> macList = new();

    public static void DownloadMacList()
    {
        string ml = Utils.WebClient.DownloadString(
            "https://gist.githubusercontent.com/aallan/b4bb86db86079509e6159810ae9bd3e4/raw/846ae1b646ab0f4d646af9115e47365f4118e5f6/mac-vendor.txt");

        string[] lines = ml.Replace("\r", "").Split('\n');
        foreach (string line in lines)
        {
            string[] parts = line.Split('\t', 2);
            if (parts.Length > 1)
            {
                string mac = parts[0].Trim();
                string vendor = parts[1].Trim();
                macList[mac] = vendor;
            }
        }
    }

    private static string MacAddressToString(byte[] macAdrr)
    {
        string macString = BitConverter.ToString(macAdrr);
        return macString.ToUpper();
    }

    private static void ThreadedARPRequest(string ipString, ref List<(string ip, string mac, string info)> result)
    {
        IPAddress ipAddress = new(0);
        byte[] macAddr = new byte[6];

        try
        {
            ipAddress = IPAddress.Parse(ipString);
            SendARP(BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0), 0, macAddr, ref macAddrLen);
            if (MacAddressToString(macAddr) != "00-00-00-00-00-00")
            {
                string macString = MacAddressToString(macAddr);
                result.Add(new(ipString, macString, GetDeviceInfoFromMac(macString)));
            }
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("Error reading IP address", ConsoleColor.Red);
        }
    }

    private static string GetDeviceInfoFromMac(string mac)
    {
        try
        {
            string firstPart = mac.Replace("-", "").Substring(0, 6);
            if (macList.ContainsKey(firstPart))
                return macList[firstPart];
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("Error reading MAC address list", ConsoleColor.Red);
        }

        return "Unknown";
    }

    public static List<(string ip, string mac, string info)> ScanAddressRange(IEnumerable<string> addresses)
    {
        List<(string ip, string mac, string info)> result = new();

        try
        {
            foreach (string ipString in addresses)
            {
                Thread threadARP = new(() => ThreadedARPRequest(ipString, ref result));
                threadARP.Start();
            }
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine("[red]Error reading IP addresses from file[/]");
        }

        Thread.Sleep(4000); // Default timeout
        return result;
    }
}