using System.Runtime.InteropServices;
using CommandLine;
using ManagedNativeWifi;
using Spectre.Console;

namespace ip.commands;

[Verb("refresh", HelpText = "Refreshes windows network list")]
public class RefreshCommand : IExecutable
{
    public void Execute()
    {
        NativeWifi.ScanNetworksAsync(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
        AnsiConsole.MarkupLine("[green]Network list refreshed[/]");
    }
}