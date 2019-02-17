using System;
using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Connection
{
	public class HelloMessage : IMessage
	{

		public const short MessageId = 3;
		public short Id => MessageId;


		public HelloMessage() { }

		public void Serialize(BinaryWriter writer)
		{

		}

		public void Deserialize(BinaryReader reader)
		{

		}

	}
}
