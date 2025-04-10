using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public class Weapon
    {
        [Key]
        public short ItemId { get; set; }

        public string Name { get; set; }
        public string AdditionalName { get; set; }
        public string BodyPart { get; set; }
        public string Crystallizable { get; set; }
        public short Weight { get; set; }
        public byte Soulshots { get; set; }
        public byte Spiritshots { get; set; }
        public string Material { get; set; }
        public string CrystalType { get; set; }
        public short PDam { get; set; }
        public short RndDam { get; set; }
        public string WeaponType { get; set; }
        public short Critical { get; set; }
        public sbyte HitModify { get; set; }
        public sbyte AvoidModify { get; set; }
        public short ShieldDef { get; set; }
        public short ShieldDefRate { get; set; }
        public short AtkSpeed { get; set; }
        public byte MpConsume { get; set; }
        public short MDam { get; set; }
        public int Duration { get; set; }
        public int Time { get; set; }
        public int Price { get; set; }
        public short CrystalCount { get; set; }
        public string Sellable { get; set; }
        public string Dropable { get; set; }
        public string Destroyable { get; set; }
        public string Tradeable { get; set; }
        public string Depositable { get; set; }
        public short Enchant4SkillId { get; set; }
        public byte Enchant4SkillLvl { get; set; }
        public short OnCastSkillId { get; set; }
        public byte OnCastSkillLvl { get; set; }
        public short OnCastSkillChance { get; set; }
        public short OnCritSkillId { get; set; }
        public byte OnCritSkillLvl { get; set; }
        public short OnCritSkillChance { get; set; }
        public short ChangeWeaponId { get; set; }
        public string Skill { get; set; }
    }
}
