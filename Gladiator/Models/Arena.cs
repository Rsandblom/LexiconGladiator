using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gladiator.Models
{
	public class Arena
	{
		[NotMapped]
		public static int BaseStartHp { get; set; } = 5;
	
		[Key]
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public int Difficulty { get; private set; } = 1;

		public ICollection<Fight> Fights { get; private set; }
		
		private Arena() { }
		public Arena(string name, int difficulty)
		{
			Name = name;
			Difficulty = difficulty;
		}
	}
}
