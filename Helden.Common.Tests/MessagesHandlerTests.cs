using System.Linq;
using Helden.Common.Network.Protocol;
using Helden.Common.Network.Protocol.Messages.Basic;
using Xunit;

namespace Helden.Common.Tests
{
    public class MessagesHandlerTests
    {

        [Fact]
        public void SerializeDeserializeMessage()
        {
            MessagesManager.Initialize();
            var pingMsg = new PingMessage(123456789);

            // Length(int) + Id(short) + Time(long)
            byte[] serializedData = MessagesManager.SerializeMessage(pingMsg);
            Assert.Equal(4 + 2 + 8, serializedData.Length);

            byte[] dataWithoutLength = serializedData.Skip(4).ToArray();
            var deserializedPingMsg = (PingMessage)MessagesManager.GetDeserializedMessage(dataWithoutLength);
            Assert.Equal(123456789, deserializedPingMsg.Time);
        }

    }
}
