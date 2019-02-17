using System;
using System.Net;
using System.Net.Sockets;

namespace HeldenServer.Network
{
    /// <summary>
    /// This class handles an instance of TcpListener.
    /// It accepts clients and triggers an event when they are connected/disconnected
    /// or when an error occurs.
    /// It also disposes the ClientWrapper when the client is disconnected.
    /// </summary>
    public class ServerWrapper
    {

        #region Fields

        private readonly TcpListener _tcpListener;

        #endregion

        #region Properties

        public bool Listening { get; private set; }

        #endregion

        #region Events

        public event Action<ClientWrapper> ClientConnected;
        public event Action<ClientWrapper> ClientDisconnected;
        public event Action<Exception> ErrorOccured;

        #endregion

        public ServerWrapper(IPAddress ipAddress, int port)
        {
            _tcpListener = new TcpListener(ipAddress, port);
        }

        #region Public Methods

        public void Start()
        {
            if (Listening)
                return;

            try
            {
                _tcpListener.Start();
                Listening = true;

                _tcpListener.BeginAcceptSocket(TcpListener_AcceptCallback, _tcpListener);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        public void Stop()
        {
            if (!Listening)
                return;

            try
            {
                _tcpListener.Stop();
                Listening = false;
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        #endregion

        #region Private Methods

        private void TcpListener_AcceptCallback(IAsyncResult ar)
        {
            if (!Listening)
                return;

            try
            {
                var newClient = (ar.AsyncState as TcpListener).EndAcceptSocket(ar);
                var clientWrapper = new ClientWrapper(newClient);
                clientWrapper.Disconnected += Client_Disconnected;
                clientWrapper.Start();
                ClientConnected?.Invoke(clientWrapper);

                // Re-call BeginAcceptSocket
                _tcpListener.BeginAcceptSocket(TcpListener_AcceptCallback, _tcpListener);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        private void Client_Disconnected(ClientWrapper client)
        {
            try
            {
                ClientDisconnected?.Invoke(client);
                client.Dispose();
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(ex);
            }
        }

        #endregion

    }
}
