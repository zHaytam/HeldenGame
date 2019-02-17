using System;
using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Connection
{
	public class ClientVersionMessage : IMessage
	{

		public const short MessageId = 4;
		public short Id => MessageId;
		public string Version { get; set; }

		public ClientVersionMessage() { }

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(Version);
		}

		public void Deserialize(BinaryReader reader)
		{
			Version = reader.ReadString();
		}

	}
}
