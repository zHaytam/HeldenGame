using System;
using System.IO;

namespace Helden.Common.Network.Protocol.Messages.Connection
{
	public class WelcomeMessage : IMessage
	{

		public const short MessageId = 6;
		public short Id => MessageId;


		public WelcomeMessage() { }

		public void Serialize(BinaryWriter writer)
		{

		}

		public void Deserialize(BinaryReader reader)
		{

		}

	}
}
