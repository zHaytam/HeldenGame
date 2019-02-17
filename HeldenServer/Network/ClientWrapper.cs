using System;
using System.Net.Sockets;
using System.Threading;

namespace HeldenServer.Network
{
    /// <summary>
    /// This class handles an instance of Socket, it's used to handle a connected client.
    /// It will trigger events when the client is connected/disconnected,
    /// when the client receives data (parsed) and when an error occurs.
    /// </summary>
    public class ClientWrapper : IDisposable
    {

        #region Fields

        private static int _counter;
        private Socket _socket;
        private byte[] _buffer;
        private MessagesParser _parser;

        #endregion

        #region Properties

        public int Id { get; }

        public bool Running { get; private set; }

        public bool IsConnected => Running && _socket?.Connected == true;

        #endregion

        #region Events

        public event Action<ClientWrapper> Connected;
        public event Action<ClientWrapper, byte[]> DataReceived;
        public event Action<ClientWrapper, Exception> ErrorOccured;
        public event Action<ClientWrapper> Disconnected;

        #endregion

        public ClientWrapper(Socket socket)
        {
            _socket = socket;
            _buffer = new byte[8192];

            _parser = new MessagesParser();
            _parser.MessageParsed += Parser_MessageParsed;

            Id = Interlocked.Increment(ref _counter);
        }

        public ClientWrapper() : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) { }

        #region Public Methods

        public void Connect(string host, int port)
        {
            if (Running || _socket.Connected)
                return;

            _socket.BeginConnect(host, port, Socket_ConnectCallback, _socket);
        }

        public void Start()
        {
            Running = true;

            try
            {
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Socket_ReceiveCallback, _socket);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }
        }

        public void Close()
        {
            if (!Running)
                return;

            try
            {
                _socket.Close();
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }
        }

        public void Send(byte[] data)
        {
            if (!Running || data.Length == 0)
                return;

            try
            {
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, Socket_SendCallback, _socket);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }
        }

        #endregion

        #region Private Methods

        private void Socket_ConnectCallback(IAsyncResult ar)
        {
            try
            {
                (ar.AsyncState as Socket).EndConnect(ar);
                Running = true;
                Connected?.Invoke(this);

                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Socket_ReceiveCallback, _socket);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }
        }

        private void Socket_ReceiveCallback(IAsyncResult ar)
        {
            if (!Running)
                return;

            try
            {
                var socket = ar.AsyncState as Socket;

                if (!socket.Connected)
                {
                    OnDisconnected();
                    return;
                }

                var availableData = socket.EndReceive(ar);

                // If the available data is 0, it means that the client disconnected
                if (availableData == 0)
                {
                    OnDisconnected();
                    return;
                }

                var data = new byte[availableData];
                Array.Copy(_buffer, data, availableData);
                _parser.HandleData(data);

                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, Socket_ReceiveCallback, _socket);
            }
            catch (SocketException)
            {
                OnDisconnected();
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }
        }

        private void Socket_SendCallback(IAsyncResult ar)
        {
            if (!Running)
                return;

            try
            {
                (ar.AsyncState as Socket).EndSend(ar);
            }
            catch (Exception ex)
            {
                ErrorOccured?.Invoke(this, ex);
            }
        }

        private void Parser_MessageParsed(byte[] data)
        {
            if (!Running)
                return;

            DataReceived?.Invoke(this, data);
        }

        private void OnDisconnected()
        {
            Running = false;
            Disconnected?.Invoke(this);
        }

        #endregion

        public void Dispose()
        {
            Running = false;

            _socket.Dispose();
            _parser.Dispose();
            Array.Clear(_buffer, 0, _buffer.Length);

            _socket = null;
            _parser = null;
            _buffer = null;
        }

    }
}
