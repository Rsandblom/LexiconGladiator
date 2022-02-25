using Gladiator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.ViewModels.PlayerGladiators

{
    public class GearShopViewModel
    {
        public int Id { get; set; }
        public int Gold { get; set; }
        public List<GearShopModifiersViewModel> AllModifiers { get; set; }

        public List<GearShopModifiersViewModel> GladiatorModifiers { get; set; }
    }
}
