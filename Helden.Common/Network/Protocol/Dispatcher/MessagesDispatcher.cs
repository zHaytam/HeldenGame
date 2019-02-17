using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Helden.Common.Network.Protocol.Messages;

namespace Helden.Common.Network.Protocol.Dispatcher
{
    public class MessagesDispatcher<T> where T : IDispatcherSource
    {

        #region Fields

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, RegisteredMessage<T>>> _registeredMessages
            = new ConcurrentDictionary<Type, ConcurrentDictionary<int, RegisteredMessage<T>>>();
        private readonly T _source;

        #endregion

        public MessagesDispatcher(T source)
        {
            _source = source;
        }

        #region Public Methods

        /// <summary>
        /// Dispatches the message to all registered actions.
        /// </summary>
        /// <param name="message">The message to dispatch.</param>
        public void DispatchMessage(IMessage message)
        {
            var registeredMsgs = GetRegisteredMessages(message.GetType());
            if (registeredMsgs == null)
                return;

            foreach (var rm in registeredMsgs)
            {
                rm.Handler.Invoke(_source, message);
            }
        }

        /// <summary>
        /// Registers an action that will get invoked whenever that message is received.
        /// </summary>
        /// <typeparam name="TMsg">The type of the message.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="priority">The priority of this registration.</param>
        public void RegisterMessage<TMsg>(Action<T, TMsg> action, MessagePriority priority) where TMsg : IMessage
        {
            // Check if the Message Type was never used
            var msgType = typeof(TMsg);
            if (!_registeredMessages.ContainsKey(msgType))
                _registeredMessages.TryAdd(msgType, new ConcurrentDictionary<int, RegisteredMessage<T>>());

            if (_registeredMessages.TryGetValue(msgType,
                out ConcurrentDictionary<int, RegisteredMessage<T>> registeredMsgs))
            {
                // Check if the same method/action is already registered
                int actionHashCode = action.GetHashCode();
                if (registeredMsgs.ContainsKey(actionHashCode))
                    throw new Exception("The same method/action is already registered.");

                // If not, add it
                registeredMsgs.TryAdd(actionHashCode, new RegisteredMessage<T>
                {
                    Handler = (s, o) => action(s, (TMsg)o),
                    Priority = priority
                });
            }
        }

        /// <summary>
        /// Unregisters a previously registered action.
        /// </summary>
        /// <typeparam name="TMsg">The type of the message.</typeparam>
        /// <param name="action">The previously registered action.</param>
        public void UnregisterMessage<TMsg>(Action<T, TMsg> action)
        {
            // Check if the Message Type exists
            var msgType = typeof(TMsg);
            if (_registeredMessages.TryGetValue(msgType,
                out ConcurrentDictionary<int, RegisteredMessage<T>> registeredMsgs))
            {
                // Try and remove the registered message
                registeredMsgs.TryRemove(action.GetHashCode(), out _);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets all the registered messages of a Message's Type.
        /// </summary>
        /// <param name="msgType">The type of the message.</param>
        /// <returns>Registered messages if found, otherwise null.</returns>
        private IEnumerable<RegisteredMessage<T>> GetRegisteredMessages(Type msgType)
        {
            if (_registeredMessages.TryGetValue(msgType,
                out ConcurrentDictionary<int, RegisteredMessage<T>> registeredMsgs))
            {
                return registeredMsgs.Values.OrderByDescending(rm => (int)rm.Priority);
            }

            return null;
        }

        #endregion

    }
}
