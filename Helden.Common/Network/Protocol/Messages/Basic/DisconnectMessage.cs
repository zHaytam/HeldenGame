using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Basic
{
	public class DisconnectMessage : IMessage
	{

		public const short MessageId = 7;
		public short Id => MessageId;
		public DisconnectReason Reason { get; set; }

		public DisconnectMessage() { }

		public void Serialize(BinaryWriter writer)
		{
			writer.Write((byte)Reason);
		}

		public void Deserialize(BinaryReader reader)
		{
			Reason = (DisconnectReason)reader.ReadByte();
		}

	}
}
