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
using System.IO;

namespace L2CSharP.Network.Gameserver.Packets
{
    public abstract class SendBasePacket
    {
        private MemoryStream vStream;

        private GameService vClient;
        public SendBasePacket(GameService client)
        {
            vStream = new MemoryStream();
            vClient = client;
        }

        protected void WriteBytes(byte[] value)
        {
            vStream.Write(value, 0, value.Length);
        }

        protected void WriteBytes(byte[] value, int Offset, int Length)
        {
            vStream.Write(value, Offset, Length);
        }

        protected void WriteInteger(int value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        protected void WriteShort(short value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        protected void WriteByte(byte value)
        {
            vStream.WriteByte(value);
        }

        protected void WriteDouble(double value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        protected void WriteString(string value)
        {
            if (!(value == null))
            {
                WriteBytes(System.Text.Encoding.Unicode.GetBytes(value));
            }
            vStream.WriteByte(0);
            vStream.WriteByte(0);
        }

        protected void WriteLong(long value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public byte[] ToByteArray()
        {
            return vStream.ToArray();
        }

        public long Length
        {
            get { return vStream.Length; }
        }

        public GameService Client
        {
            get { return vClient; }
        }

        protected internal abstract void Write();
    }
}