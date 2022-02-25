using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gladiator.Models
{
	public class Modifiers
    {
        /// <summary>
        /// Modifies stats, might want to add placement to avoid 13 helmets and such
        /// </summary>
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Str { get; set; }
        public int Hp { get; set; }
        public int Xp { get; set; }
        public int Def { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public GearType TypeOfGear { get; set; }
        public Fighter Fighter { get; set; }
        public int FighterId { get; set; }

        public Modifiers()
        {

        }

        public Modifiers(string name, int str, int hp, int xp, int def)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Str = str;
            this.Hp = hp;
            this.Xp = xp;
            this.Def = def;
        }

        public Modifiers(string name, int str, int hp, int xp, int def, string description)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Str = str;
            this.Hp = hp;
            this.Xp = xp;
            this.Def = def;
            Description = description;
        }

        public Modifiers ShallowCopy()
        {
            return (Modifiers)MemberwiseClone();
        }
    }
}
