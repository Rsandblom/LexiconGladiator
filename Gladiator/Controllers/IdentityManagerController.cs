using Gladiator.Data;
using Gladiator.Models;
using Gladiator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IdentityManagerController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GladiatorContext _gladiatorContext;

        public IdentityManagerController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, GladiatorContext gladiatorContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _gladiatorContext = gladiatorContext;
        }

        public async Task<IActionResult> Index()
        {
            IdentityManagerViewModel identityMngVM = new IdentityManagerViewModel();
            var roles = await _roleManager.Roles.ToListAsync();
            identityMngVM.RolesList = roles;

            var users = _userManager.Users.ToList();
            var userRolesViewModelList = new List<UserWithRolesViewModel>();

            foreach (ApplicationUser user in users)
            {
                var userViewModel = new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = await GetUserRoles(user)
                };
                userRolesViewModelList.Add(userViewModel);
            }
            identityMngVM.UserWithRolesList = userRolesViewModelList;

            return View(identityMngVM);
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        /*
        [HttpPost]
        public async Task<IActionResult> Create(IdentityManagerViewModel identityManagerViewModel)
        {
            if (identityManagerViewModel.Name != null)
            {
                await _roleManager.CreateAsync(new IdentityRole(identityManagerViewModel.Name.Trim()));
            }

            return RedirectToAction(nameof(Index));
        }

        */
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _roleManager.Roles.ToListAsync();
            var userPlayer = _gladiatorContext.UserGladiator.Where(up => up.ApplicationUserId == user.Id).Include(u => u.Fighters).FirstOrDefault();

            IdentityManagerEditViewModel identityMngEditVM = new IdentityManagerEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                FightersList = userPlayer.Fighters
            };

            if (userPlayer != null)
            {
                identityMngEditVM.PlayerName = userPlayer.Name;
                identityMngEditVM.Gold = userPlayer.Gold;
            }
            else
            {
                identityMngEditVM.PlayerName = "";
            }

            identityMngEditVM.CreateRolesSelectList(roles);

            return View(identityMngEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, IdentityManagerEditViewModel identityManagerEditVM)
        {
            if (id != identityManagerEditVM.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            var role = await _roleManager.FindByIdAsync(identityManagerEditVM.RoleIdString);
            var userPlayer = _gladiatorContext.UserGladiator.Where(up => up.ApplicationUserId == user.Id).FirstOrDefault();

            if (user == null)
            {
                return View();
            }

            user.FirstName = identityManagerEditVM.FirstName;
            user.LastName = identityManagerEditVM.LastName;
            user.Email = identityManagerEditVM.Email;

            if (userPlayer != null)
            {
                userPlayer.Name = identityManagerEditVM.PlayerName;
                userPlayer.Gold = identityManagerEditVM.Gold;
                _gladiatorContext.SaveChanges();
            }
            
            

            await _userManager.AddToRoleAsync(user, role.Name);
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, IdentityManagerEditViewModel identityManagerEditVM)
        {
            if (id != identityManagerEditVM.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            var role = await _roleManager.FindByIdAsync(identityManagerEditVM.RoleIdString);
            var userPlayer = _gladiatorContext.UserGladiator.Where(up => up.ApplicationUserId == user.Id).FirstOrDefault();

            if (user == null)
            {
                return View();
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            if (userPlayer != null)
            {
                _gladiatorContext.Remove(userPlayer);
                _gladiatorContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
