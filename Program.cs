using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using CommandLine;
using ip.commands;
using Spectre.Console;

namespace ip;

public static class Program
{
    public static void Main(string[] args) =>
        Parser.Default.ParseArguments<CheckCommand, TraceCommand, InfoCommand, RefreshCommand, ScanCommand>(args)
            .WithParsed<IExecutable>(x => x.Execute());
}