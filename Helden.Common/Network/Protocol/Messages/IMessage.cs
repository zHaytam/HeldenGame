using System.IO;

namespace Helden.Common.Network.Protocol.Messages
{
    public interface IMessage
    {

        /// <summary>
        /// The message's identifier.
        /// </summary>
        short Id { get; }

        /// <summary>
        /// Here, each message should write its properties.
        /// The Id property should NOT be written.
        /// </summary>
        /// <param name="writer">The binary writer</param>
        void Serialize(BinaryWriter writer);
        
        /// <summary>
        /// Here, each message should read its properties.
        /// The Id property should NOT be read.
        /// </summary>
        /// <param name="reader"></param>
        void Deserialize(BinaryReader reader);

    }
}
