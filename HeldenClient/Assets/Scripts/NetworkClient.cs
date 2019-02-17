using System;
using System.Collections;
using Assets.Scripts.Handlers;
using Helden.Common.Network;
using Helden.Common.Network.Protocol;
using Helden.Common.Network.Protocol.Dispatcher;
using Helden.Common.Network.Protocol.Messages;
using Helden.Common.Network.Protocol.Messages.Basic;
using Telepathy;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Handles a Telepathy client instance.
    /// </summary>
    public class NetworkClient : SingletonBehaviour<NetworkClient>, IDispatcherSource
    {

        #region Fields

        [Header("Server Informations")]
        [SerializeField] private string _ip;
        [SerializeField] private int _port;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _connectionStateText;

        private Client _client;
        private ConnectionState _state;

        #endregion

        #region Properties

        public ConnectionState State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
                OnStateChanged();
            }
        }

        public MessagesDispatcher<NetworkClient> Dispatcher { get; private set; }

        #endregion

        #region Events

        public event Action<ConnectionState> StateChanged;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // update even if window isn't focused, otherwise we don't receive.
            Application.runInBackground = true;

            // Load messages
            MessagesManager.Initialize();
            DontDestroyOnLoad(this);

            // Telepathy logs
            Telepathy.Logger.Log = Debug.Log;
            Telepathy.Logger.LogWarning = Debug.LogWarning;
            Telepathy.Logger.LogError = Debug.LogError;

            _client = new Client();
            Dispatcher = new MessagesDispatcher<NetworkClient>(this);
        }

        private void Start() => Connect();

        private void Update()
        {
            while (_client.GetNextMessage(out Message message))
            {
                switch (message.eventType)
                {
                    case Telepathy.EventType.Connected:
                        OnConnected();
                        break;
                    case Telepathy.EventType.Data:
                        OnDataReceived(message.data);
                        break;
                    case Telepathy.EventType.Disconnected:
                        OnDisconnected();
                        break;
                }
            }
        }

        private void OnApplicationQuit()
        {
            // the client/server threads won't receive the OnQuit info if we are
            // running them in the Editor. they would only quit when we press Play
            // again later. this is fine, but let's shut them down here for consistency
            _client.Disconnect();
        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            State = ConnectionState.Connecting;
            _client.Connect(_ip, _port);
        }

        public bool SendMessage(IMessage message)
        {
            if (!_client.Connected)
                return false;

            byte[] data = MessagesManager.SerializeMessage(message, includeLength: false);
            return _client.Send(data);
        }

        #endregion

        #region Private Methods

        private void OnConnected()
        {
            HandlersManager.Initialize();
            State = ConnectionState.Connected;
        }

        private void OnDataReceived(byte[] data)
        {
            var message = MessagesManager.GetDeserializedMessage(data);
            if (message == null)
            {
                Debug.Log("ERROR: GetDeserializedMessage returned null");
                return;
            }

            Dispatcher.DispatchMessage(message);
            Debug.Log($"Received & Dispatched {message.GetType().Name}");
        }

        private void OnDisconnected()
        {
            HandlersManager.Clean();
            State = ConnectionState.Disconnected;
        }

        private void OnStateChanged()
        {
            _connectionStateText.text = State.ToString();
            StateChanged?.Invoke(State);
        }

        #endregion

    }
}
