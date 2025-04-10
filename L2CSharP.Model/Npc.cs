using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class Npc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }
        public int CollisionRadius { get; set; }
        public int CollisionHeight { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int AttackRange { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int HpReg { get; set; }
        public int MpReg { get; set; }
        public int Str { get; set; }
        public int Con { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }
        public int Wit { get; set; }
        public int Men { get; set; }
        public int Exp { get; set; }
        public int Sp { get; set; }
        public int Patk { get; set; }
        public int Pdef { get; set; }
        public int Matk { get; set; }
        public int Mdef { get; set; }
        public int AtkSpd { get; set; }
        public int Aggro { get; set; }
        public int MatkSpd { get; set; }
        public int Rhand { get; set; }
        public int Lhand { get; set; }
        public int Enchant { get; set; }
        public int WalkSpd { get; set; }
        public int RunSpd { get; set; }
        public int DropHerbGroup { get; set; }
        public int BaseStats { get; set; }
        public int IsAttackable { get; set; }

        public virtual ICollection<SpawnList> Spawns { get; set; }
    }

}
