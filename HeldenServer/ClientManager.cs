using Helden.Common.Network;
using Helden.Common.Network.Protocol;
using Helden.Common.Network.Protocol.Dispatcher;
using Helden.Common.Network.Protocol.Messages;
using Helden.Common.Network.Protocol.Messages.Basic;
using HeldenServer.Network;
using Serilog;

namespace HeldenServer
{
    /// <summary>
    /// ClientManager will manage a single connected client.
    /// </summary>
    public class ClientManager : IDispatcherSource
    {

        #region Fields

        private readonly ClientWrapper _client;

        #endregion

        #region Properties

        public int Id => _client.Id;

        public MessagesDispatcher<ClientManager> Dispatcher { get; }

        public bool IsConnected => _client.IsConnected;

        #endregion

        public ClientManager(ClientWrapper client)
        {
            _client = client;
            _client.DataReceived += ClientDataReceived;

            Dispatcher = new MessagesDispatcher<ClientManager>(this);
        }

        #region Public Methods

        public void SendMessage(IMessage message)
        {
            if (!IsConnected)
                return;

            byte[] data = MessagesManager.SerializeMessage(message);
            _client.Send(data);
        }

        public void Disconnect(DisconnectReason reason)
        {
            SendMessage(new DisconnectMessage
            {
                Reason = reason
            });
            _client.Close();
        }

        #endregion

        #region Event Handlers

        private void ClientDataReceived(ClientWrapper client, byte[] data)
        {
            var message = MessagesManager.GetDeserializedMessage(data);
            if (message == null)
            {
                Log.Error($"GetDeserializedMessage returned null. Client N°{Id}.");
                return;
            }

            Log.Debug($"Received {message.GetType().Name} from Client N°{Id}.");
            Dispatcher.DispatchMessage(message);
        }

        #endregion

    }
}
