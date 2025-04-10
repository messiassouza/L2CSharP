using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{

    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int ObjId { get; set; }
        public string Username { get; set; }
        public string CharacterName { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public string Title { get; set; }
        public int CurHP { get; set; }
        public int CurMP { get; set; }
        public int CurCP { get; set; }
        public int SP { get; set; }
        public int PkKills { get; set; }
        public int PvpKills { get; set; }
        public int Gender { get; set; }
        public int ClassId { get; set; }
        public int Online { get; set; }
        public int Karma { get; set; }
        public int HairColor { get; set; }
        public int HairStyle { get; set; }
        public int Face { get; set; }
        public int Heading { get; set; }
        public int IsGM { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int IsHero { get; set; }
        public int IsNoble { get; set; }

        [ForeignKey("Clan")]
        public long? ClanId { get; set; }
        public virtual Clan Clan { get; set; }

        public int Race { get; set; }

        public virtual ICollection<CharacterItem> Items { get; set; }
        public virtual ICollection<CharacterShortcut> Shortcuts { get; set; }
        public virtual ICollection<CharacterSkill> Skills { get; set; }
    }
}
