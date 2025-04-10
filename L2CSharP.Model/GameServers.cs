using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public  class GameServers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int Port { get; set; }
        public int IsTestServer { get; set; }
        public int Brackets  { get; set; }
        public string ServerHash { get; set; }
        public bool IsOnline { get; set; }
        public int CurrentPlayers { get; set; }
    }
}
