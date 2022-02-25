using Gladiator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gladiator.ViewModels
{
	public class IdentityManagerEditViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First name")]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [Display(Name = "Email adress")]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public string RoleIdString { get; set; }

        [Display(Name = "Player name")]
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string PlayerName { get; set; }

        [Display(Name = "Gold")]
        [Required]
        public int Gold { get; set; }

        public List<Fighter> FightersList { get; set; }

        private List<SelectListItem> _roles;
        public List<SelectListItem> Roles { get => _roles; }

        public void CreateRolesSelectList(List<IdentityRole> roleList)
        {
            List<SelectListItem> rolesSelectList = new List<SelectListItem>();
            foreach (var role in roleList)
            {
                rolesSelectList.Add(new SelectListItem { Value = role.Id.ToString(), Text = role.Name });
            }
            _roles = rolesSelectList;
        }
    }
}
