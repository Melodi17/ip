using CommandLine;
using Spectre.Console;

namespace ip.commands;

[Verb("scan", HelpText = "Scan for devices on subnet")]
public class ScanCommand : IExecutable
{
    public void Execute()
    {
        string? localIp = Utils.GetLocalIPAddress();
        if (localIp == null)
        {
            AnsiConsole.MarkupLine("[red]Could not determine local IP[/]");
            return;
        }

        AnsiConsole.MarkupLine($"Scanning subnet for devices on {localIp}");

        IEnumerable<string> GetAddressRange()
        {
            string cut = localIp[..localIp.LastIndexOf('.')];
            for (int i = 1; i < 255; i++)
                yield return $"{cut}.{i}";
        }

        ArpScanner.DownloadMacList();
        List<(string ip, string mac, string info)> results = ArpScanner.ScanAddressRange(GetAddressRange());
        
        AnsiConsole.MarkupLine($"Found [cyan]{results.Count}[/] devices");
        Table table = new();
        table.AddColumn("IP");
        table.AddColumn("MAC");
        table.AddColumn("Manufacturer");
        
        table.Border(TableBorder.Rounded);
        
        foreach (var (ip, mac, info) in results)
            table.AddRow(ip, mac, info);
        
        AnsiConsole.Write(table);
    }
}