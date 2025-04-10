using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Packets
{
    public abstract class ReceiveBasePacket : IReceivePacket
    {
        private readonly byte[] _packet;
        private int _offset;
        private readonly GameClient _client;
        private readonly IGameLogger _logger;

        protected ReceiveBasePacket(GameClient client, byte[] packet, IGameLogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _packet = packet ?? throw new ArgumentNullException(nameof(packet));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _offset = 1; // Typically opcode is at position 0
        }

        public int Offset
        {
            get => _offset;
            set => _offset = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public GameClient Client => _client;
        public byte[] Packet => _packet;

        public abstract void Read();
        public abstract void Run();

        protected int ReadInteger()
        {
            ValidateRead(4);
            var result = BitConverter.ToInt32(_packet, _offset);
            _offset += 4;
            return result;
        }

        protected byte ReadByte()
        {
            ValidateRead(1);
            var result = _packet[_offset];
            _offset += 1;
            return result;
        }

        protected byte[] ReadBytes(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            ValidateRead(length);
            var result = new byte[length];
            Buffer.BlockCopy(_packet, _offset, result, 0, length);
            _offset += length;
            return result;
        }

        protected short ReadShort()
        {
            ValidateRead(2);
            var result = BitConverter.ToInt16(_packet, _offset);
            _offset += 2;
            return result;
        }

        protected double ReadDouble()
        {
            ValidateRead(8);
            var result = BitConverter.ToDouble(_packet, _offset);
            _offset += 8;
            return result;
        }

        protected string ReadString()
        {
            try
            {
                ValidateRead(2); // Minimum 2 bytes for empty string with null terminator

                // Find null terminator
                int nullTerminatorIndex = -1;
                for (int i = _offset; i < _packet.Length - 1; i += 2)
                {
                    if (_packet[i] == 0 && _packet[i + 1] == 0)
                    {
                        nullTerminatorIndex = i;
                        break;
                    }
                }

                int stringLength = (nullTerminatorIndex - _offset) / 2;
                if (stringLength < 0) stringLength = 0;

                var result = Encoding.Unicode.GetString(_packet, _offset, stringLength * 2);
                _offset += (stringLength * 2) + 2; // +2 for null terminator

                return result;
            }
            catch (Exception ex)
            {
                _logger.Debug($"Error while reading string from packet: {ex.Message}");
                return string.Empty;
            }
        }

        private void ValidateRead(int bytesToRead)
        {
            if (_offset + bytesToRead > _packet.Length)
            {
                throw new InvalidOperationException(
                    $"Attempt to read beyond packet bounds. Offset: {_offset}, Length: {bytesToRead}, Packet size: {_packet.Length}");
            }
        }
    }
}
