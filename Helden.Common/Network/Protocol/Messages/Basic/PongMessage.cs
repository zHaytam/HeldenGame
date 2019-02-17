using System;
using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Basic
{
    public class PongMessage : IMessage
    {

        public const short MessageId = 2;
        public short Id => MessageId;
        public long Time { get; set; }

        public PongMessage()
        {
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public PongMessage(long time)
        {
            Time = time;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Time);
        }

        public void Deserialize(BinaryReader reader)
        {
            Time = reader.ReadInt64();
        }

    }
}
