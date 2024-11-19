using CommandLine;
using Spectre.Console;

namespace ip.commands;

[Verb("info", isDefault: true, HelpText = "Show info")]
public class InfoCommand : IExecutable
{
    public void Execute()
    {
        string? localIp = Utils.GetLocalIPAddress();
        IPInfo? publicIpInfo = Utils.GetPublicIPInfo();
        bool hasNetwork = Utils.HasNetwork();
        bool internet = Utils.IsInternetAvailable();

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine($"Local IP ......: [cyan]{localIp ?? "[grey]<unknown>[/]"}[/]");
        AnsiConsole.MarkupLine($"Public IP .....: [cyan]{publicIpInfo?.ip ?? "[grey]<unknown>[/]"}[/]");
        AnsiConsole.MarkupLine($"Location ......: [cyan]{(publicIpInfo != null ? $"{publicIpInfo.CountryFriendly}, {publicIpInfo.region}" : "[grey]<unknown>[/]")}[/]");
        AnsiConsole.MarkupLine($"Organization ..: [cyan]{publicIpInfo?.org ?? "[grey]<unknown>[/]"}[/]");
        AnsiConsole.MarkupLine($"Network .......: {(hasNetwork ? "[green]yes" : "[red]no")}[/]");
        AnsiConsole.MarkupLine($"Internet ......: {(internet ? "[green]yes" : "[red]no")}[/]");
    }
}