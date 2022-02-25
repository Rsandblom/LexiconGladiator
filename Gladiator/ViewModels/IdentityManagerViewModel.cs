using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gladiator.ViewModels
{
    public class IdentityManagerViewModel
    {

        public List<IdentityRole> RolesList { get; set; }
        [Display(Name="Role name")]
        public string Name { get; set; }

        public List<UserWithRolesViewModel> UserWithRolesList { get; set; }

        


    }
}
