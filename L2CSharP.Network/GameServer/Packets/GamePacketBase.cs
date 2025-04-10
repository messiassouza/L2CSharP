using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Gameserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.GameServer.Packets
{

    /// <summary>
    /// Classe base abstrata para todos os pacotes do servidor de jogos
    /// </summary>
    public abstract class GamePacketBase
    {
        public short Opcode { get; protected set; }
        public string Name { get; protected set; }
        protected GameService Client { get; }
        protected byte[] PacketData { get; }
        protected int Offset { get; set; }
        protected IGameLogger Logger { get; }

        protected GamePacketBase(GameService client, byte[] packet, IGameLogger logger)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            PacketData = packet ?? throw new ArgumentNullException(nameof(packet));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Offset = 2; // Pula o opcode (2 bytes)
        }

        public abstract void Read();
        public abstract void Run();

        protected byte[] ReadBytes(int count)
        {
            var data = new byte[count];
            Buffer.BlockCopy(PacketData, Offset, data, 0, count);
            Offset += count;
            return data;
        }

        protected int ReadInt32()
        {
            int value = BitConverter.ToInt32(PacketData, Offset);
            Offset += 4;
            return value;
        }

        protected short ReadShort() => BitConverter.ToInt16(ReadBytes(2), 0);
        protected int ReadInt() => BitConverter.ToInt32(ReadBytes(4), 0);
        protected byte ReadByte() => ReadBytes(1)[0];
        protected bool ReadBool() => Convert.ToBoolean(ReadByte());
        protected string ReadString() => Encoding.UTF8.GetString(ReadBytes(ReadShort()));
    }

}
