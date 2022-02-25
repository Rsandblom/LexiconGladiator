using Gladiator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.ViewModels.PlayerGladiators
{
    public class GearShopModifiersViewModel
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
        
        public int FighterId { get; set; }

        public GearShopModifiersViewModel()
        {

        }


        public GearShopModifiersViewModel ShallowCopy()
        {
            return (GearShopModifiersViewModel)MemberwiseClone();
        }
    }
}

