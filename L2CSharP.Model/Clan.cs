using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class Clan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int ClanId { get; set; }
        public string ClanName { get; set; }
        public int RepScore { get; set; }
        public int LeaderObjectId { get; set; }
        public int CrestId { get; set; }
        public int ClanLevel { get; set; }
        public int AllianceId { get; set; }
        public string AllianceName { get; set; }
        public int AllianceCrestId { get; set; }

        [InverseProperty("Clan")]
        public virtual ICollection<Character> Members { get; set; }
    }
}
