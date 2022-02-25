using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Gladiator.Models
{
	public class Fighter
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Str { get; set; }
        public int Hp { get; set; }
        public int Xp { get; set; }
        public int Def { get; set; }

        [NotMapped]
        public int CalculatedStrenght { get; set; }
        [NotMapped]
        public int CalculatedHealtPoints { get; set; }
        [NotMapped]
        public int CalculatedExperiencePoints { get; set; }
        [NotMapped]
        public int CalculatedDefence { get; set; }


        public bool IsDeleted { get; set; }

        public bool IsOpponent { get; set; }

        public int HeadModifierId { get; set; }
        [NotMapped]
        public Modifiers Head { get; set; }
        public int BodyModifierId { get; set; }
        [NotMapped]
        public Modifiers Body { get; set; }
        public int LeftHandModifierId { get; set; }
        [NotMapped]
        public Modifiers LeftHand { get; set; }
        public int RightHandModifierId { get; set; }
        [NotMapped]
        public Modifiers RightHand { get; set; }

        public List<Modifiers> Modifiers { get ; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        List<Modifiers> modifiers;
        public static List<Fighter> gladiatorCharList;

        public ICollection<Fight> ChallengerFights { get; set; }
        public ICollection<Fight> AdversaryFights { get; set; }
        public ICollection<Fight> WinnerFights { get; set; }
        

        public Fighter(string name, int str, int hp, int xp, int def, List<Modifiers> modifiers)
        {
            this.Name = name;
            this.Str = str;
            this.Hp = hp;
            this.Xp = xp;
            this.Def = def;
            this.modifiers = modifiers;
        }

        public Fighter(string name, int str, int hp, int xp, int def)
        {
            this.Name = name;
            this.Str = str;
            this.Hp = hp;
            this.Xp = xp;
            this.Def = def;
        }

        public Fighter()
            : this (names[new Random().Next(0, 99)], 
                  new Random().Next(0, 9), 
                  new Random().Next(0, 9), 
                  new Random().Next(0, 9), 
                  new Random().Next(0, 9))
        {         
        }

        public Fighter GetFighter()
        {
            return new Fighter(this.Name, 
                this.Str + modifiers.Sum(x => x.Str),
                this.Hp + modifiers.Sum(x => x.Hp),
                this.Xp + modifiers.Sum(x => x.Xp),
                this.Def + modifiers.Sum(x => x.Def));
        }

        public void CalculateStatsWithPlacedModifiers()
        {
            CalculatedStrenght = Str + Head.Str + Body.Str + RightHand.Str + LeftHand.Str;
            CalculatedHealtPoints = Hp + Head.Hp + Body.Hp + RightHand.Hp + LeftHand.Hp;
            CalculatedExperiencePoints = Xp + Head.Xp + Body.Xp + RightHand.Xp + LeftHand.Xp;
            CalculatedDefence = Def + Head.Def + Body.Def + RightHand.Def + LeftHand.Def;
        }

        public void SetModifiers()
        {
            Head = Modifiers.Where(m => m.Id == HeadModifierId).FirstOrDefault();
            Body = Modifiers.Where(m => m.Id == BodyModifierId).FirstOrDefault();
            RightHand = Modifiers.Where(m => m.Id == RightHandModifierId).FirstOrDefault();
            LeftHand = Modifiers.Where(m => m.Id == LeftHandModifierId).FirstOrDefault();
        }

        public Fighter ShallowCopy()
        {
            return (Fighter)MemberwiseClone();
        }

        
        public string PictureSrc()
        {
            return (this.Id % 6) +".png";
        }

        static string[] names = {
            "Abhivira",
            "Achilles",
            "Agnar",
            "Agro",
            "Alcinder",
            "Alexander",
            "Alfonso",
            "Aloysius",
            "Armand",
            "Aryan",
            "Bade",
            "Bahaadur",
            "Batair",
            "Bertrand",
            "Bhaltair",
            "Boris",
            "Cadel",
            "Callan",
            "Casey",
            "Chad",
            "Clancy",
            "Clovis",
            "Cyrus",
            "Daljit",
            "Denzel",
            "Dorian",
            "Duncan",
            "Durand",
            "Dustin",
            "Earl",
            "Einar",
            "Eloy",
            "Evander",
            "Finley",
            "Gautier",
            "Gerald",
            "Gideon",
            "Gunnar",
            "Gunther",
            "Griffith",
            "Harbin",
            "Harold",
            "Harvey",
            "Herman",
            "Horatius",
            "Humphrey",
            "Igor",
            "Ivor",
            "Jabbar",
            "Jaivira",
            "Jerry",
            "Jimmu",
            "Julius",
            "Kaiden",
            "Kane",
            "Karamveer",
            "Kijani",
            "Kimball",
            "Koa",
            "Liam",
            "Louis",
            "Ludwig",
            "Luther",
            "Malou",
            "Marcel",
            "Marcus",
            "Martin",
            "Milo",
            "Mordecai",
            "Murdock",
            "Murphy",
            "Nakoa",
            "Nolan",
            "Oscar",
            "Owen",
            "Patton",
            "Peyton",
            "Ragnar",
            "Rainer",
            "Ranjit",
            "Roger",
            "Rollo",
            "Ryder",
            "Sacha",
            "Saladin",
            "Saxon",
            "Sibbi",
            "Sloan",
            "Sweeney",
            "Swithin",
            "Thor",
            "Troy",
            "Umberto",
            "Viggo",
            "Veerle",
            "Walt",
            "Warner",
            "Warrick",
            "Wyatt",
            "Zander"
        };
    }

}
