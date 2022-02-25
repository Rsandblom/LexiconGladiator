using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gladiator.Data;
using Gladiator.Models;
using Gladiator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Gladiator.ViewModels.AdminFighters;

namespace Gladiator.Controllers
{
	[Authorize(Roles = "Admin")]
    public class AdminFightersController : Controller
    {
        private readonly GladiatorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminFightersController(GladiatorContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Gladiators.ToListAsync()); 
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(f => f.Id == id);

            if (fighter == null)
            {
                return NotFound();
            }
            
            fighter.SetModifiers();
            fighter.CalculateStatsWithPlacedModifiers();
            return View(fighter);
        }

        public IActionResult Create()
        {
            CreateFighterViewModel createFighterVM = new CreateFighterViewModel();
            
            return View(createFighterVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFighterViewModel createFighterViewModel)
        {
            var applicationUser = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            applicationUser.Player = await _context.UserGladiator.Where(u => u.ApplicationUserId == applicationUser.Id).FirstOrDefaultAsync();

            Fighter fighter = new Fighter
            {
                Name = createFighterViewModel.Name,
                Str = createFighterViewModel.Str,
                Hp = createFighterViewModel.Hp,
                Xp = createFighterViewModel.Xp,
                Def = createFighterViewModel.Def,
                IsDeleted = false,
                IsOpponent = createFighterViewModel.IsOpponent,
                UserId = applicationUser.Player.Id
            };

            _context.Add(fighter);
            await _context.SaveChangesAsync();
            Modifiers headModifier = new Modifiers { Id = 0, Name = "Leather cap", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Hardened leather cap", Price = 10, TypeOfGear = GearType.Head, FighterId = fighter.Id };
            Modifiers bodyModifiers = new Modifiers { Id = 0, Name = "Cotton Shirt", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Plain cotton shirt", Price = 10, TypeOfGear = GearType.Body, FighterId = fighter.Id };
            Modifiers rightHandModifiers = new Modifiers { Id = 0, Name = "Wooden stick", Str = 1, Hp = 0, Xp = 0, Def = 0, Description = "Simple stick of wood", Price = 10, TypeOfGear = GearType.RightHand, FighterId = fighter.Id };
            Modifiers leftHandModifiers = new Modifiers { Id = 0 , Name = "Wooden shield", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Shield made of wood", Price = 10, TypeOfGear = GearType.LeftHand, FighterId = fighter.Id };

            List<Modifiers> modifiers = new List<Modifiers>();
            modifiers.Add(headModifier);
            modifiers.Add(bodyModifiers);
            modifiers.Add(rightHandModifiers);
            modifiers.Add(leftHandModifiers);
            fighter.Modifiers = modifiers;
            await _context.SaveChangesAsync();

            fighter.HeadModifierId = headModifier.Id;
            fighter.BodyModifierId = bodyModifiers.Id;
            fighter.RightHandModifierId = rightHandModifiers.Id;
            fighter.LeftHandModifierId = leftHandModifiers.Id;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddModifiers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.FindAsync(id);
            if (fighter == null)
            {
                return NotFound();
            }

            AddModifiersViewModel addModifiersToFighterVM = new AddModifiersViewModel();
            addModifiersToFighterVM.Modifiers = await _context.Modifiers.Where(m => m.FighterId == 1).ToListAsync();
            addModifiersToFighterVM.Id = fighter.Id;
            var modifiersList = await _context.Modifiers.Where(m => m.FighterId == 1).ToListAsync();
            addModifiersToFighterVM.CreateModifiersSelectList(modifiersList);
            addModifiersToFighterVM.Modifiers = await _context.Modifiers.Where(m => m.FighterId == fighter.Id).ToListAsync();

            return View(addModifiersToFighterVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddModifiers(int? id, AddModifiersViewModel addModifiersToFighterVM)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(f => f.Id == id);
            if (fighter == null)
            {
                return NotFound();
            }

            var modifiersList = _context.Modifiers.Where(m => m.FighterId == 1).ToList();
            var addedModifier = modifiersList.Where(m => m.Id == addModifiersToFighterVM.ModifiersId).FirstOrDefault().ShallowCopy();
            addedModifier.Id = 0;
            fighter.Modifiers.Add(addedModifier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(f => f.Id == id);
            if (fighter == null)
            {
                return NotFound();
            }


            EditFightersViewModel editOpponentVM = new EditFightersViewModel
            {
                Id = fighter.Id,
                Name = fighter.Name,
                Str = fighter.Str,
                Hp = fighter.Hp,
                Xp = fighter.Xp,
                Def = fighter.Def,
                HeadModifierId = fighter.HeadModifierId,
                BodyModifierId = fighter.BodyModifierId,
                RightHandModifierId = fighter.RightHandModifierId,
                LeftHandModifierId = fighter.LeftHandModifierId,
                IsDeleted = fighter.IsDeleted
            };

            editOpponentVM.CreateHeadSelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.Head).ToList());
            editOpponentVM.CreateBodySelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.Body).ToList());
            editOpponentVM.CreateRightHandSelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.RightHand).ToList());
            editOpponentVM.CreateLeftHandSelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.LeftHand).ToList());
            return View(editOpponentVM);
        }
      
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditFightersViewModel editFighterViewModel)
        {
            if (id != editFighterViewModel.Id)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.FindAsync(id);
            if (fighter == null)
            {
                return NotFound();
            }

            // TODO use autoMapper instead
            fighter.Name = editFighterViewModel.Name;
            fighter.Str = editFighterViewModel.Str;
            fighter.Hp = editFighterViewModel.Hp;
            fighter.Xp = editFighterViewModel.Xp;
            fighter.Def = editFighterViewModel.Def;
            fighter.HeadModifierId = editFighterViewModel.HeadModifierId;
            fighter.BodyModifierId = editFighterViewModel.BodyModifierId;
            fighter.RightHandModifierId = editFighterViewModel.RightHandModifierId;
            fighter.LeftHandModifierId = editFighterViewModel.LeftHandModifierId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fighter == null)
            {
                return NotFound();
            }

            return View(fighter);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fighter = await _context.Gladiators.FindAsync(id);
            _context.Gladiators.Remove(fighter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FighterExists(int id)
        {
            return _context.Gladiators.Any(e => e.Id == id);
        }
    }
}
