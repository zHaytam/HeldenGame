using System;
using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Basic
{
    public class PingMessage : IMessage
    {

        public const short MessageId = 1;
        public short Id => MessageId;
        public long Time { get; set; }

        public PingMessage()
        {
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public PingMessage(long time)
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
