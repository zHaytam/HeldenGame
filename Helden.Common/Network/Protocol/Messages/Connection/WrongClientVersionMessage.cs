using System;
using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Connection
{
	public class WrongClientVersionMessage : IMessage
	{

		public const short MessageId = 5;
		public short Id => MessageId;


		public WrongClientVersionMessage() { }

		public void Serialize(BinaryWriter writer)
		{

		}

		public void Deserialize(BinaryReader reader)
		{

		}

	}
}
