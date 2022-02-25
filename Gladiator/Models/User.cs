using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gladiator.Models
{
	public class User
    {
        private string name;

        [Key]
        public int Id { get; set; }
        public string Name { get => name; set => name = value; }
        public int Gold { get; set; }
        public string ApplicationUserId { get; set; }  // user is what we call Player, hence user id to connect "player" to Application user.
        public ApplicationUser ApplicationUser { get; set; }    //Registered user
        public List<Fighter> Fighters { get; set; } // To connect user with their fighters in DB
		public static object Identity { get; internal set; }

		/// <summary>
		/// Separated ID generation for testing purposes
		/// </summary>
		/// <returns>The next unique ID</returns>
		/// 

		/*
		public int GetNextID()
		{
			 return id++;
		}

		*/

		public User()
        {
                
        }

        /*
        public User(string Name)
        {
            this.name = Name;
            this.Id = GetNextID();
        }
        */
        /*
        public void GladiatorGroup() => fighters.AddRange(new List<Fighter>
        {
            new Fighter { Id = 1, Name ="Mercurius", statsId = 1 },
            new Fighter { Id = 2, Name="Trinity", statsId = 2 },
            new Fighter { Id = 3, Name="Leviathan", statsId = 3 },
            new Fighter { Id = 4, Name="Oreo", statsId = 4 },
            new Fighter { Id = 5, Name="Ronin", statsId = 5, },
            new Fighter { Id = 6, Name="Azimuth", statsId = 6, }
        });
        */

        /*
        public void AddFighter(Fighter f)
        {
            fighters.Add(f);
        }
        */
    }
}
