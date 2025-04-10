// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
// 
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Collections.Generic;
using System.Text;


namespace L2CSharP.Network.Gameserver.Packets
{
    using L2CSharP.LoggerApi.Logger.Interfaces;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Text;

    public abstract class ReceiveBasePacket : IDisposable
    {
        protected readonly byte[] PacketData;
        protected int Offset;
        protected readonly GameService Client;
        protected readonly IGameLogger Logger;
        private bool _disposed;

        protected ReceiveBasePacket(GameService client, byte[] packet, IGameLogger logger)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            PacketData = packet ?? throw new ArgumentNullException(nameof(packet));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Offset = 1; // Assume que o primeiro byte é o opcode

            try
            {
                Read();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error during packet reading: {ex.Message}");
                throw;
            }
        }

        public int ReadInteger()
        {
            ValidateRead(4);
            var result = BitConverter.ToInt32(PacketData, Offset);
            Offset += 4;
            return result;
        }

        public byte ReadByte()
        {
            ValidateRead(1);
            var result = PacketData[Offset];
            Offset += 1;
            return result;
        }

        public byte[] ReadBytes(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive");

            ValidateRead(length);
            var result = new byte[length];
            Buffer.BlockCopy(PacketData, Offset, result, 0, length);
            Offset += length;
            return result;
        }

        public short ReadShort()
        {
            ValidateRead(2);
            var result = BitConverter.ToInt16(PacketData, Offset);
            Offset += 2;
            return result;
        }

        public double ReadDouble()
        {
            ValidateRead(8);
            var result = BitConverter.ToDouble(PacketData, Offset);
            Offset += 8;
            return result;
        }

        public string ReadString()
        {
            try
            {
                ValidateRead(2); // Mínimo para string vazia com terminador

                int nullTerminatorIndex = FindNullTerminator();
                int stringLength = nullTerminatorIndex >= 0 ?
                    (nullTerminatorIndex - Offset) / 2 :
                    (PacketData.Length - Offset) / 2;

                var result = Encoding.Unicode.GetString(PacketData, Offset, stringLength * 2);
                Offset += (stringLength * 2) + 2; // +2 para o terminador nulo

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error reading string from packet: {ex.Message}");
                return string.Empty;
            }
        }

        private int FindNullTerminator()
        {
            for (int i = Offset; i < PacketData.Length - 1; i += 2)
            {
                if (PacketData[i] == 0 && PacketData[i + 1] == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ValidateRead(int bytesToRead)
        {
            if (Offset + bytesToRead > PacketData.Length)
            {
                throw new InvalidOperationException(
                    $"Attempt to read beyond packet bounds. Offset: {Offset}, Length: {bytesToRead}, Packet size: {PacketData.Length}");
            }
        }

        public abstract void Read();
        public abstract void Run();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Liberar recursos gerenciados se necessário
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}