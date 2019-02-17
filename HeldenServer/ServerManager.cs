using System;
using System.Collections.Concurrent;
using System.Net;
using HeldenServer.Handlers;
using HeldenServer.Network;
using Serilog;

namespace HeldenServer
{
    /// <summary>
    /// ServerManager will manage the one & only server instance.
    /// </summary>
    public static class ServerManager
    {

        #region Fields

        private const int Port = 8654;
        private static readonly ServerWrapper Server;
        private static readonly ConcurrentDictionary<int, ClientManager> ClientManagers;

        #endregion

        static ServerManager()
        {
            Server = new ServerWrapper(IPAddress.Any, Port);
            Server.ClientConnected += ServerClientConnected;
            Server.ClientDisconnected += ServerClientDisconnected;
            Server.ErrorOccured += ServerErrorOccured;

            ClientManagers = new ConcurrentDictionary<int, ClientManager>();
        }

        #region Public Methods

        public static void Start()
        {
            if (Server.Listening)
                return;

            Server.Start();
            Log.Information($"Server started at port {Port}.");
        }

        public static void Stop()
        {
            if (!Server.Listening)
                return;

            Server.Stop();
            Log.Warning("Server stopped.");
        }

        #endregion

        #region Event Handlers

        private static void ServerClientConnected(ClientWrapper client)
        {
            var clientManager = new ClientManager(client);
            if (ClientManagers.TryAdd(client.Id, clientManager))
            {
                HandlersManager.HandleClient(clientManager);
                Log.Information($"Client N°{client.Id} is now being managed.");
                return;
            }

            Log.Error($"Couldn't manage client N°{client.Id}.");
        }

        private static void ServerErrorOccured(Exception ex)
        {
            // Todo: Handle this better
            Log.Error(ex, "Server Error");
        }

        private static void ServerClientDisconnected(ClientWrapper client)
        {
            if (ClientManagers.TryRemove(client.Id, out ClientManager clientManager))
            {
                HandlersManager.UnhandleClient(clientManager);
                Log.Information($"Client N°{client.Id} removed.");
                return;
            }

            Log.Error($"Client N°{client.Id} doesn't have a manager.");
        }

        #endregion

    }
}
