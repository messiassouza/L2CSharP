using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class CharacterSkill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Character")]
        public long CharId { get; set; }
        public virtual Character Character { get; set; }

        public int SkillId { get; set; }
        public int SkillLevel { get; set; }
        public int EnchantLevel { get; set; }
    }
}
