using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Gladiator.Controllers
{
	[Authorize]
	public class VenueController: Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

