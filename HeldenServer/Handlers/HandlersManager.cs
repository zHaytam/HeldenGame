using System;
using System.Collections.Generic;
using System.Reflection;

namespace HeldenServer.Handlers
{
    public static class HandlersManager
    {

        #region Fields

        private static readonly Type HandlerType = typeof(IHandler);
        private static readonly List<IHandler> Handlers = new List<IHandler>();

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Should only be called once.
        /// </summary>
        public static void Initialize()
        {
            if (Handlers.Count > 0)
                return;

            foreach (var type in Assembly.GetAssembly(HandlerType).GetTypes())
            {
                if (!HandlerType.IsAssignableFrom(type) || type == HandlerType)
                    continue;

                Handlers.Add((IHandler)Activator.CreateInstance(type));
            }
        }

        public static void HandleClient(ClientManager clientManager)
        {
            Handlers.ForEach(h => h.HandleClient(clientManager));
        }

        public static void UnhandleClient(ClientManager clientManager)
        {
            Handlers.ForEach(h => h.UnhandleClient(clientManager));
        }

        #endregion

    }
}
