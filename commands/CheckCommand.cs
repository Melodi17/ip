using CommandLine;
using Spectre.Console;

namespace ip.commands;

[Verb("check", HelpText = "Check status")]
public class CheckCommand : IExecutable
{
    [Option('p', "port", Required = true, HelpText = "Port to check")]
    public int Port { get; set; }

    [Option('i', "ip", Required = false, HelpText = "IP to check, if not provided public IP will be used")]
    public string? IP { get; set; }

    public void Execute()
    {
        string? publicIp = Utils.GetPublicIPAddress();

        string? ip = this.IP ?? publicIp;
        if (ip == null)
        {
            AnsiConsole.MarkupLine("[red]No IP provided and public IP could not be determined[/]");
            return;
        }

        AnsiConsole.MarkupLine(
            $"Port [cyan]{this.Port}[/] on [yellow]{ip}[/] is {(IsPortOpen(ip, this.Port) ? "[green]open" : "[red]closed")}[/]");
    }

    private static bool IsPortOpen(string host, int port)
    {
        if (!Utils.IsInternetAvailable()) return false;

        string url = $"https://ismyportopen.com//processors/port-check.php?ip={host}&port={port}";

        string response = Utils.WebClient.DownloadString(url);
        return response == "1";
    }
}