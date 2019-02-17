using System;
using System.IO;
using System.Linq;
using Helden.Common.Network.Protocol;
using HeldenServer.Database;
using HeldenServer.Handlers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace HeldenServer
{
    internal class Program
    {

        #region Properties

        public static IConfigurationRoot Configuration { get; private set; }

        #endregion

        private static void Main(string[] args)
        {
            // Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File("Logs/Log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Configuration
            Configuration = GetConfiguration();
            Log.Information("Configuration loaded.");

            // Messages
            MessagesManager.Initialize();
            Log.Information($"{MessagesManager.MessagesCount} messages loaded.");

            // Handlers
            HandlersManager.Initialize();
            Log.Information($"{HandlersManager.HandlersCount} handlers loaded.");

            // Start server
            Log.Debug("Starting server...");
            ServerManager.Start();

            Console.ReadKey();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("configuration.json");

            return builder.Build();
        }

    }
}
