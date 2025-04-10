using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class CharacterShortcut
    {
        [Key]
        [Column(Order = 0)]
        public long CharId { get; set; }

        [Key]
        [Column(Order = 1)]
        public long Slot { get; set; }

        [Key]
        [Column(Order = 2)]
        public long Page { get; set; }

        public int Type { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public int CharType { get; set; }

        [ForeignKey("CharId")]
        public Character Character { get; set; } 
    }
}
