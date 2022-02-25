using System;

namespace Gladiator.DTOs
{
	public class FightDTO
	{

		public Guid FightId { get; set; }
		public ArenaData Arena { get; set; }

		public FighterData You { get; set; }
		public FighterData Opponent { get; set; }


		public class ArenaData
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public int Difficulty { get; set; }
		}

		public class FighterData
		{
			public int FighterId { get; set; }
			public int UserId { get; set; }
			public string Name { get; set; }
			public int BaseStrength { get; set; }
			public int Strength { get; set; }
			public int BaseDefence { get; set; }
			public int Defence { get; set; }
			public int Xp { get; set; }
			public int StartHp { get; set; }
			public int CurrentHp { get; set; }
			public int CurrentHpPercent { get; set; }
			public bool IsComputer { get; set; }

		}

	}
}
