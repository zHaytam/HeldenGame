using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Helden.Common.Network.Protocol.Messages;
using Helden.Common.Utils.Extensions;

namespace Helden.Common.Network.Protocol
{
    public static class MessagesManager
    {

        #region Fields

        private static readonly Type MessageType = typeof(IMessage);
        private static readonly Dictionary<short, Func<IMessage>> MessagesCtors = new Dictionary<short, Func<IMessage>>();

        #endregion

        #region Properties

        public static int MessagesCount => MessagesCtors.Count;

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            // Already initialized?
            if (MessagesCount > 0)
                return;

            foreach (var type in Assembly.GetAssembly(MessageType).GetTypes())
            {
                if (!MessageType.IsAssignableFrom(type) || type == MessageType)
                    continue;

                var idField = type.GetField("MessageId");
                if (idField == null)
                    throw new Exception($"{type.Name} is an IMessage but doesn't have a MessageId const field.");

                short id = (short)idField.GetValue(null);
                if (MessagesCtors.ContainsKey(id))
                    throw new Exception($"Duplicate message: {type.Name}");

                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                    throw new Exception($"Parameterless constructor not found in '{type.Name}'");

                MessagesCtors.Add(id, ctor.CreateDelegate<Func<IMessage>>());
            }
        }

        /// <summary>
        /// Gets an instance of a certain message (deserialized).
        /// </summary>
        /// <param name="data">The bytes data.</param>
        /// <returns>An IMessage instance.</returns>
        public static IMessage GetDeserializedMessage(byte[] data)
        {
            // Doesn't even contain the id
            if (data.Length < 2)
                return null;

            IMessage message = null;
            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                short id = reader.ReadInt16();
                if (MessagesCtors.ContainsKey(id))
                {
                    message = MessagesCtors[id]();
                    message.Deserialize(reader);
                }
            }

            return message;
        }

        /// <summary>
        /// Serializes the message into an array of bytes.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <param name="includeLength">Wheither to include the length or not.</param>
        /// <returns>An array of bytes</returns>
        public static byte[] SerializeMessage(IMessage message, bool includeLength = true)
        {
            if (message == null)
                return null;
            
            // Serialize message
            byte[] messageData;
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(message.Id);
                    message.Serialize(writer);
                }

                messageData = ms.ToArray();
            }

            if (!includeLength)
                return messageData;

            // Add length of the message at the beginning
            // Todo: Think of a better way
            byte[] data = new byte[4 + messageData.Length];
            byte[] lengthData = NetworkUtils.IntToBytesBigEndian(messageData.Length);

            // Length
            int i;
            for (i = 0; i < 4; i++)
            {
                data[i] = lengthData[i];
            }

            // Data
            for (i = 0; i < messageData.Length; i++)
            {
                data[i + 4] = messageData[i];
            }

            return data;
        }

        #endregion

    }
}
