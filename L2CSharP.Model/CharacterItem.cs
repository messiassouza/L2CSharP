using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class CharacterItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public int ItemId { get; set; }
        public int ObjId { get; set; }
        public int IsEquipped { get; set; }
        public int EnchantLevel { get; set; }
        public int AugmentId { get; set; }
        public int Mana { get; set; }
        public int TimeLimit { get; set; }

        [ForeignKey("Character")]
        public long PlayerObjId { get; set; }
        public Character Character { get; set; }

        public int ItemCount { get; set; }
    }

}
