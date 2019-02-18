using Helden.Common.Network;
using Helden.Common.Network.Protocol.Dispatcher;
using Helden.Common.Network.Protocol.Messages.Connection;
using Serilog;

namespace HeldenServer.Handlers.Connection
{
    public class ConnectionHandler : IHandler
    {

        private const string Version = "0.1";

        public void HandleClient(ClientManager clientManager)
        {
            clientManager.Dispatcher.RegisterMessage<ClientVersionMessage>(HandleClientVersionMessage, MessagePriority.High);
            clientManager.SendMessage(new HelloMessage());
        }

        #region Messages

        private void HandleClientVersionMessage(ClientManager clientManager, ClientVersionMessage message)
        {
            Log.Debug($"Client N°{clientManager.Id} has version {message.Version}.");

            if (message.Version == Version)
            {
                clientManager.SendMessage(new WelcomeMessage());
            }
            else
            {
                clientManager.SendMessage(new WrongClientVersionMessage());
                clientManager.Disconnect(DisconnectReason.WrongVersion);
            }
        }

        #endregion

        public void UnhandleClient(ClientManager clientManager)
        {
            clientManager.Dispatcher.UnregisterMessage<ClientVersionMessage>(HandleClientVersionMessage);
        }

    }
}
