using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gladiator.Data;
using Gladiator.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gladiator.Controllers
{
	[Authorize(Roles = "Admin")]
    public class ModifiersController : Controller
    {
        private readonly GladiatorContext _context;

        public ModifiersController(GladiatorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Modifiers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Str,Hp,Xp,Def,Description,Price,TypeOfGear")] Modifiers modifiers)
        {
            if (ModelState.IsValid)
            {
                modifiers.FighterId = 1; //FighterId 1 is the Modifiers FighterId of all Modifiers created by Admin/seeded
                _context.Add(modifiers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(modifiers);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modifiers = await _context.Modifiers.FindAsync(id);
            if (modifiers == null)
            {
                return NotFound();
            }
            return View(modifiers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Str,Hp,Xp,Def,Description,Price,TypeOfGear,FighterId")] Modifiers modifiers)
        {
            if (id != modifiers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modifiers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModifiersExists(modifiers.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(modifiers);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modifiers = await _context.Modifiers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (modifiers == null)
            {
                return NotFound();
            }

            return View(modifiers);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modifiers = await _context.Modifiers.FindAsync(id);
            _context.Modifiers.Remove(modifiers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModifiersExists(int id)
        {
            return _context.Modifiers.Any(e => e.Id == id);
        }
    }
}
