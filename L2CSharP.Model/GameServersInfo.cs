using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{

    public class GameServersInfo
    {
        public short ServerID { get; set; }
        public string InternalIP { get; set; }
        public string ExternalIP { get; set; }
        public string ServerHash { get; set; }
        public short Port { get; set; }
        public short AgeLimit { get; set; }
        public bool Pvp { get; set; }
        public short CurPlayers { get; set; }
        public short MaxPlayers { get; set; }
        public bool Online { get; set; }
        public bool ShowClock { get; set; }
        public bool Brackets { get; set; }
        public bool IsOnline { get; set; }
        public int CurrentPlayers { get; set; }
        public object Name { get; internal set; }

        public GameServersInfo(short serverID, string LanIP, string WanIP, short port, short ageLimit, bool pvp,
                               short curPlayers, short maxPlayers, bool online, bool showClock,
                               bool brackets, string serverHash)
        {
            ServerID = serverID;
            InternalIP = LanIP;
            ExternalIP = WanIP;
            Port = port;
            AgeLimit = ageLimit;
            Pvp = pvp;
            CurPlayers = curPlayers;
            MaxPlayers = maxPlayers;
            Online = online;
            ShowClock = showClock;
            Brackets = brackets;
            ServerHash = serverHash;
        }
    }

}
