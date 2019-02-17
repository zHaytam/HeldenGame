using System;

namespace Helden.Common.Network.Protocol.Dispatcher
{
    public class RegisteredMessage<T> where T : IDispatcherSource
    {

        #region Properties

        public MessagePriority Priority { get; set; }

        public Action<T, object> Handler { get; set; }

        #endregion

    }
}
