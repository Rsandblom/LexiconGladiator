using AutoMapper;
using Gladiator.Data;
using Gladiator.Models;
using Gladiator.ViewModels;
using Gladiator.ViewModels.PlayerGladiators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.Controllers
{
	public class PlayerGladiatorsController : Controller
    {
        private readonly GladiatorContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public PlayerGladiatorsController(GladiatorContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var indexVM = new PlayerGladiatorsIndexViewModel();
            var gladiatorList = await _context.Gladiators.Where(g => g.IsOpponent == false).ToListAsync();
            var applicationUser = await _context.Users.Include(u => u.Player).Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefaultAsync();

            indexVM.PlayersGladiatorsList = gladiatorList.Where(g => g.UserId == applicationUser.Player.Id).ToList(); //TODO Maybe exlude IsDeleted gladiators
            indexVM.StandardGladiatorsList = gladiatorList.Where(g => g.UserId == 1).ToList(); //Temp solution with Admin Id

            return View(indexVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(m => m.Id == id);

            if (fighter == null)
            {
                return NotFound();
            }

            fighter.SetModifiers();
            fighter.CalculateStatsWithPlacedModifiers();

            return View(fighter);
        }

        public async Task<IActionResult> Recruit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(m => m.Id == id);

            if (fighter == null)
            {
                return NotFound();
            }

            fighter.SetModifiers();
            fighter.CalculateStatsWithPlacedModifiers();

            return View(fighter);
        }

        [HttpPost]
        public async Task<IActionResult> Recruit(int id)
        {
            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(f => f.Id == id);

            //Add fighter to players collection
            Fighter recuitedFighter = new Fighter();
            recuitedFighter = fighter.ShallowCopy();
            var applicationUser = await _context.Users.Include(u => u.Player).Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefaultAsync();
            recuitedFighter.UserId = applicationUser.Player.Id;
            recuitedFighter.Id = 0;

            //Add the fighters/gladiators modifers to DB with shallowcopy and set their FightId to Id of player
            List<Modifiers> modifers = new List<Modifiers>();

            var headMod = fighter.Modifiers.Where(m => m.Id == fighter.HeadModifierId).FirstOrDefault().ShallowCopy();
            headMod.Id = 0;
            var bodyMod = fighter.Modifiers.Where(m => m.Id == fighter.BodyModifierId).FirstOrDefault().ShallowCopy();
            bodyMod.Id = 0;
            var rightHandMod = fighter.Modifiers.Where(m => m.Id == fighter.RightHandModifierId).FirstOrDefault().ShallowCopy();
            rightHandMod.Id = 0;
            var leftHandMod = fighter.Modifiers.Where(m => m.Id == fighter.LeftHandModifierId).FirstOrDefault().ShallowCopy();
            leftHandMod.Id = 0;

            modifers.Add(headMod);
            modifers.Add(bodyMod);
            modifers.Add(rightHandMod);
            modifers.Add(leftHandMod);

            recuitedFighter.Modifiers = modifers;

            await _context.Gladiators.AddAsync(recuitedFighter);
            await _context.SaveChangesAsync();
            recuitedFighter.HeadModifierId = headMod.Id;
            recuitedFighter.BodyModifierId = bodyMod.Id;
            recuitedFighter.RightHandModifierId = rightHandMod.Id;
            recuitedFighter.LeftHandModifierId = leftHandMod.Id;
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
            fighter.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GearShop(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(m => m.Id == id);

            if (fighter == null)
            {
                return NotFound();
            }

            fighter.SetModifiers();
            fighter.CalculateStatsWithPlacedModifiers();


            return View(fighter);
        }

        public async Task<IActionResult> GearDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modifiers = await _context.Modifiers.FirstOrDefaultAsync(m => m.Id == id);
            if (modifiers == null)
            {
                return NotFound();
            }

            return View(modifiers);
        }

        [HttpGet]
        public async Task<IActionResult> GetModifiers(int id)
        {

            GearShopViewModel gearShopVM = new GearShopViewModel();
            var applicationUser = await _context.Users.Include(u => u.Player)
                .ThenInclude(p => p.Fighters)
                .ThenInclude(f => f.Modifiers)
                .Where(u => u.Id == _userManager.GetUserId(User))
                .FirstOrDefaultAsync();
            var allModifiers = await _context.Modifiers.Where(f => f.FighterId == 1).ToListAsync();

            gearShopVM.Id = id;
            gearShopVM.Gold = applicationUser.Player.Gold;

            List<GearShopModifiersViewModel> gearShopModifiersVMAll = new List<GearShopModifiersViewModel>();
            foreach (var item in allModifiers)
            {
                var editVM = _mapper.Map<GearShopModifiersViewModel>(item);
                gearShopModifiersVMAll.Add(editVM);
            }

            gearShopVM.AllModifiers = gearShopModifiersVMAll;

            var gladiator = applicationUser.Player.Fighters.Where(f => f.Id == id).FirstOrDefault();
            List<GearShopModifiersViewModel> gearShopModifiersVMPlayer = new List<GearShopModifiersViewModel>();
            foreach (var item in gladiator.Modifiers.ToList())
            {
                var editVM = _mapper.Map<GearShopModifiersViewModel>(item);
                gearShopModifiersVMPlayer.Add(editVM);
            }
            gearShopVM.GladiatorModifiers = gearShopModifiersVMPlayer;

            return Json(gearShopVM);
        }

        [HttpPost]
        public async Task<IActionResult> BuyGear([FromBody] GearShopBuyViewModel gearShopBuyVM)
        {
            if (ModelState.IsValid)
            {
                if (gearShopBuyVM.Id == null)
                {
                    return NotFound();
                }

                var fighter = await _context.Gladiators.Include(f => f.Modifiers).FirstOrDefaultAsync(f => f.Id == gearShopBuyVM.Id);
                if (fighter == null)
                {
                    return NotFound();
                }

                var modifiersList = _context.Modifiers.Where(m => m.FighterId == 1).ToList();
                var addedModifier = modifiersList.Where(m => m.Id == gearShopBuyVM.ModifierId).FirstOrDefault().ShallowCopy();
                addedModifier.Id = 0;
                fighter.Modifiers.Add(addedModifier);

                var applicationUser = await _context.Users.Include(u => u.Player).Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefaultAsync();

                var currentGold = applicationUser.Player.Gold;
                applicationUser.Player.Gold = currentGold - addedModifier.Price;

                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
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

            var editVM = _mapper.Map<EditViewModel>(fighter);

            editVM.CreateHeadSelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.Head).ToList());
            editVM.CreateBodySelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.Body).ToList());
            editVM.CreateRightHandSelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.RightHand).ToList());
            editVM.CreateLeftHandSelectList(fighter.Modifiers.Where(m => m.TypeOfGear == GearType.LeftHand).ToList());
            return View(editVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditViewModel editVM)
        {
            if (id != editVM.Id)
            {
                return NotFound();
            }

            var fighter = await _context.Gladiators.FindAsync(id);
            if (fighter == null)
            {
                return NotFound();
            }

            fighter.Name = editVM.Name;
            fighter.HeadModifierId = editVM.HeadModifierId;
            fighter.BodyModifierId = editVM.BodyModifierId;
            fighter.RightHandModifierId = editVM.RightHandModifierId;
            fighter.LeftHandModifierId = editVM.LeftHandModifierId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
