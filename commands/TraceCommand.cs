using System.Net.NetworkInformation;
using System.Text;
using CommandLine;
using Spectre.Console;

namespace ip.commands;

[Verb("trace", HelpText = "Trace route")]
public class TraceCommand : IExecutable
{
    //[Option('i', "ip", Required = false, HelpText = "IP to trace route to, if not provided public IP will be used")]
    [Value(0, MetaName = "ip", Required = true, HelpText = "IP to trace route to, if not provided public IP will be used")]
    public string IP { get; set; }
    
    public void Execute()
    {
        string? ip = this.IP;
        
        if (ip == null)
        {
            AnsiConsole.MarkupLine("[red]No IP provided and public IP could not be determined[/]");
            return;
        }

        List<TraceRouteHopInfo> result = new();

        bool dontFragment = true;
        string data = Guid.NewGuid().ToString();

        const int hopLimit = 30;

        for (int hopIndex = 0; hopIndex < hopLimit; hopIndex++)
        {
            // Setting the TTL is the heart of the traceroute principle
            int ttl = hopIndex + 1;
            int timeout = 15000; // 15 seconds.
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            Ping ping = new();
            PingOptions pingOptions = new(ttl, dontFragment);

            // Let's ping
            PingReply pingReply = ping.Send(ip, timeout, dataBytes, pingOptions);

            TraceRouteHopInfo traceRouteHopInfo = new()
                { HopIndex = hopIndex + 1, PingReply = pingReply };
            result.Add(traceRouteHopInfo);

            string displayName = Utils.GetHostName(pingReply.Address.ToString()) ?? pingReply.Address.ToString();

            AnsiConsole.MarkupLine(
                $"Hop [cyan]{traceRouteHopInfo.HopIndex}[/] - [yellow]{displayName}[/] - [green]{pingReply.RoundtripTime}ms[/]");

            if (pingReply.Status == IPStatus.Success)
                // The ping reached the destination after all hops in between.
                break;
        }
    }
}

internal record TraceRouteHopInfo
{
    public int HopIndex { get; init; }
    public PingReply PingReply { get; init; }
}