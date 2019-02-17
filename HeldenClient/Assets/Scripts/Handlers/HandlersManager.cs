using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Handlers
{
    public class HandlersManager
    {

        #region Fields

        private static readonly Type HandlerType = typeof(IHandler);
        private static readonly List<IHandler> Handlers = new List<IHandler>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Should be called everytime we connect to the server.
        /// </summary>
        public static void Initialize()
        {
            Clean();

            foreach (var type in Assembly.GetAssembly(HandlerType).GetTypes())
            {
                if (!HandlerType.IsAssignableFrom(type) || type == HandlerType)
                    continue;

                Handlers.Add((IHandler)Activator.CreateInstance(type));
            }

            Handlers.ForEach(h => h.Initialize());
        }

        /// <summary>
        /// Cleans all the handlers.
        /// </summary>
        public static void Clean()
        {
            if (Handlers.Count <= 0) 
                return;

            Handlers.ForEach(h => h.Clean());
            Handlers.Clear();
        }

        #endregion

    }
}
