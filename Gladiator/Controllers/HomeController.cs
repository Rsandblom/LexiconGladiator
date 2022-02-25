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
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;

namespace Gladiator.Controllers
{
	public class HomeController: Controller
	{
		private readonly GladiatorContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;

		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, GladiatorContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
		{
			_context = context;
			_userManager = userManager;
			_mapper = mapper;
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Highscores()
		{
			return View(_context);
        }

        public IActionResult Arena()    // Temporary for testing, until all parts have been incorporated into the main page
        {
            return View();
        }

		public IActionResult Arena1()
		{
			return View();
		}

		public IActionResult Arena2()
		{
			return View();
		}
		public IActionResult Arena3()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Selection(string challengerId, string adversaryId, string arenaId)
		{
			TempData["challengerId"] = challengerId;

			if(adversaryId == "CPU")
				TempData["adversaryId"] = 2;
			else
			{
				_ = int.TryParse(adversaryId, out int advId);
				IList<Fighter> adversaryFighters = _context.Gladiators.Where(f => f.UserId == advId).ToList();

				TempData["adversaryId"] = adversaryFighters[new Random().Next(0, adversaryFighters.Count)].Id;
			}

			switch(arenaId)
			{
				case "gA1d":
					TempData["arenaId"] = "341B68D0-6F19-4260-B056-2433F1AB49AB";
					return RedirectToAction("Arena1");

				case "gA2fl":
					TempData["arenaId"] = "7813A8BF-BA83-4D93-B0DE-4ED7F7C11C81";
					return RedirectToAction("Arena1");

				case "gA3fl":
					TempData["arenaId"] = "8EAFA150-D6D4-4909-8391-13C6C55F84D6";
					return RedirectToAction("Arena1");

				case "gA5":
						TempData["arenaId"] = "5E22F9CC-175F-41B8-8C21-13D18FE28606";
					return RedirectToAction("Arena1");

				default:
					return NoContent();
			}

		}

		public async Task<IActionResult> Selection()
		{
			var indexVM = new PlayerGladiatorsIndexViewModel();
			var gladiatorList = await _context.Gladiators.Where(g => g.IsOpponent == false).ToListAsync();
			var applicationUser = await _context.Users.Include(u => u.Player).Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefaultAsync();

			indexVM.PlayersGladiatorsList = gladiatorList.Where(g => g.UserId == applicationUser.Player.Id).ToList(); //TODO Maybe exlude IsDeleted gladiators
			indexVM.StandardGladiatorsList = gladiatorList.Where(g => g.UserId == 1).ToList(); //Temp solution with Admin Id
			indexVM.UsersList = await _context.Users.Include(u => u.Player).Where(u => u.Player.Id != 1).ToListAsync(); 
			return View(indexVM);
		}


		public IActionResult Login()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

