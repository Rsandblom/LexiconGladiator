using Microsoft.AspNetCore.Identity;

namespace Gladiator.Models
{
	public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }

        public User Player { get; set; }


    }
}
