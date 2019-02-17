using System;
using System.IO;
using Helden.Common.Network;

namespace HeldenServer.Network
{
    /// <summary>
    /// This class handles parsing messages.
    /// Each message has the following form:
    /// [Length(int, 4 bytes, BI)|Content(..., Length bytes)]
    /// MessagesParser will get bytes fed into it and it will trigger
    /// a MessageParsed event when a fully message was received (length + content).
    /// The byte[] in the MessageParsed event only contain the content!
    /// </summary>
    public class MessagesParser : IDisposable
    {

        // Fields
        private BinaryReader _reader;
        private int _currentMessageLength;


        // Properties
        private int DataLength => (int)(_reader.BaseStream.Length - _reader.BaseStream.Position);


        // Event
        public event Action<byte[]> MessageParsed;


        // Constructor
        public MessagesParser()
        {
            _reader = new BinaryReader(new MemoryStream());
        }


        public void HandleData(byte[] data)
        {
            if (data?.Length == 0)
                return;

            AddData(data);
            ProcessData();
        }

        private void ProcessData()
        {
            while (true)
            {
                // If the parser is already handling a message
                // and if the reader has all the bytes we need
                if (_currentMessageLength != 0 && DataLength >= _currentMessageLength)
                {
                    var bytes = _reader.ReadBytes(_currentMessageLength);
                    _currentMessageLength = 0;

                    MessageParsed?.Invoke(bytes);
                    continue;
                }

                // Otherwise check if we can read the message's length
                if (DataLength >= 4)
                {
                    _currentMessageLength = NetworkUtils.BytesToIntBigEndian(_reader.ReadBytes(4));
                    continue;
                }

                // If we can't read anything anymore, might aswell clear the stream
                if (DataLength == 0)
                {
                    _reader.Dispose();
                    _reader = new BinaryReader(new MemoryStream());
                }

                break;
            }
        }

        private void AddData(byte[] data)
        {
            var pos = _reader.BaseStream.Position;
            _reader.BaseStream.Position = _reader.BaseStream.Length;
            _reader.BaseStream.Write(data, 0, data.Length);
            _reader.BaseStream.Position = pos;
        }

        public void Dispose()
        {
            _reader.Dispose();
            _reader = null;
        }

    }
}
