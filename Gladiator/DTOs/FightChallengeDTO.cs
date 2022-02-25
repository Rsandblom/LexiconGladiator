using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.DTOs
{
	public class FightChallengeDTO
	{
		public Guid FightId { get; set; }

		public FighterData Challenger { get; set; }
		public FighterData Adversary { get; set; }

		public Guid ArenaId { get; set; }
		public string ArenaName { get; set; }
		public int ArenaDifficulty { get; set; }

		public class FighterData
		{
			public int FighterId { get; set; }
			public string FighterName { get; set; }
			public string FighterUserName { get; set; }
			public int FighterXp { get; set; }
			public int FighterHp { get; set; }
			public int FighterStrenght { get; set; }
			public int FighterDefence { get; set; }
		}
	}
}
