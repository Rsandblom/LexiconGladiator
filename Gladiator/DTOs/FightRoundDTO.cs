using System;

namespace Gladiator.DTOs
{
	public class FightRoundDTO
	{
		public Guid FightId { get; set; }
		public int RoundNo { get; set; }
		public bool SuccessfulDefence { get; set; }
		public FighterData You { get; set; }
		public FighterData Opponent { get; set; }
		public string RoundDescription { get; set; }

		public class FighterData
		{
			public int FighterId { get; set; }
			public bool IsAttacker { get; set; }
			public int FightStrenght { get; set; }
			public int StartHp { get; set; }
			public int HpLoss { get; set; }
			public int CurrentHp { get; set; }
			public int CurrentHpPercent { get; set; }
		}
	}
}
