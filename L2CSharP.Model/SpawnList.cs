using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class SpawnList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Location { get; set; }



        [ForeignKey("Npc")]
        public long NpcTemplateId { get; set; }
        public virtual Npc Npc { get; set; }




        public int LocX { get; set; }
        public int LocY { get; set; }
        public int LocZ { get; set; }
        public int Heading { get; set; }
        public int RespawnDelay { get; set; }
        public byte PeriodOfDay { get; set; }
    }
}
