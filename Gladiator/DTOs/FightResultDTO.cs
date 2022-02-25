using System;

namespace Gladiator.DTOs
{
	public class FightResultDTO
	{
		public Guid FightId { get; set; }
		public Guid AreanaId { get; set; }
		public int Difficulty { get; set; }
		public int NoRounds { get; set; }
		public FighterStats ChallengerStats { get; set; }
		public FighterStats AdversaryStats { get; set; }

		public class FighterStats
		{
			public int FighterId { get; set; }
			public int XpChange { get; set; }
			public int NewXp { get; set; }
			public int GoldWon { get; set; }
		}


	}
}
