using AutoMapper;
using Gladiator.Models;
using Gladiator.ViewModels.PlayerGladiators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.Profiles
{
    public class PlayerEditProfile : Profile
    {
        public PlayerEditProfile()
        {
            CreateMap<Fighter, EditViewModel>();
        }
        
    }
}
