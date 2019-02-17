using System;
using Helden.Common.Network.Protocol;
using HeldenServer.Handlers;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace HeldenServer
{
    class Program
    {

        static void Main(string[] args)
        {
            // Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File("Logs/Log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Messages & Handlers
            MessagesManager.Initialize();
            HandlersManager.Initialize();
            ServerManager.Start();

            Console.ReadKey();
        }

    }
}
