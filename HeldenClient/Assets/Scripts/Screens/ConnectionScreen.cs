using System.Runtime.CompilerServices;
using Assets.Scripts.Dialogs;
using Assets.Scripts.I18n;
using Assets.Scripts.Scenes;
using Helden.Common.Network.Protocol.Dispatcher;
using Helden.Common.Network.Protocol.Messages.Connection;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Screens
{
    /// <summary>
    /// This screen is the one responsible for connecting the player
    /// to the server and checking the versions.
    /// If everything is good, the next scene must be loaded.
    /// </summary>
    public class ConnectionScreen : MonoBehaviour
    {

        #region Fields

        [SerializeField] private TextMeshProUGUI _stateText;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            NetworkClient.Instance.StateChanged += NetworkClient_StateChanged;
            NetworkClient.Instance.Dispatcher.RegisterMessage<HelloMessage>(HandleHelloMessage, MessagePriority.High);
            NetworkClient.Instance.Dispatcher.RegisterMessage<WrongClientVersionMessage>(
                HandleWrongClientVersionMessage, MessagePriority.High);
            NetworkClient.Instance.Dispatcher.RegisterMessage<WelcomeMessage>(HandleWelcomeMessage,
                MessagePriority.High);

            // When the scene changes to ConnectionScene,
            // we might already be disconnected.
            CheckIfDisconnected();
        }

        private void OnDisable()
        {
            // Annoying error when we stop playing in the Editor
            if (NetworkClient.Instance == null)
                return;

            NetworkClient.Instance.StateChanged -= NetworkClient_StateChanged;
            NetworkClient.Instance.Dispatcher.UnregisterMessage<HelloMessage>(HandleHelloMessage);
            NetworkClient.Instance.Dispatcher.UnregisterMessage<WrongClientVersionMessage>(
                HandleWrongClientVersionMessage);
            NetworkClient.Instance.Dispatcher.UnregisterMessage<WelcomeMessage>(HandleWelcomeMessage);
        }

        #endregion

        #region Event Handlers

        private void NetworkClient_StateChanged(ConnectionState newState)
        {
            _stateText.SetText(I18NManager.Instance.GetNormal($"{newState.ToString()}State"));
            CheckIfDisconnected();
        }

        #endregion

        #region Private Methods

        private static void CheckIfDisconnected([CallerMemberName] string caller = null)
        {
            Debug.Log($"{caller} -- {NetworkClient.Instance.State}");
            if (NetworkClient.Instance.State != ConnectionState.Disconnected)
                return;

            DialogsBehaviour.Instance.ShowTouchable(I18NManager.Instance.GetNormal("ErrorTitle"),
                I18NManager.Instance.GetNormal("RetryConnection"), TryReconnecting);
        }

        private static void TryReconnecting()
        {
            NetworkClient.Instance.Connect();
        }

        #endregion

        #region Messages

        private static void HandleHelloMessage(NetworkClient client, HelloMessage message)
        {
            NetworkClient.Instance.SendMessage(new ClientVersionMessage
            {
                Version = Application.version
            });
        }

        private static void HandleWrongClientVersionMessage(NetworkClient client, WrongClientVersionMessage message)
        {
            Debug.Log("WRONG VERSION");
            // Todo: blocking dialog
        }

        private static void HandleWelcomeMessage(NetworkClient client, WelcomeMessage message)
        {
            NetworkClient.Instance.State = ConnectionState.FullyConnected;
            SceneChanger.Instance.ChangeScene(AvailableScene.Login);
        }

        #endregion

    }
}
