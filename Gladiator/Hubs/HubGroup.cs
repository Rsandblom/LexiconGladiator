using System;

namespace Gladiator.Hubs
{
	public class HubGroup
	{
		public static string FighterGroup(int fighterId)
		{
			return $"Fighter: {fighterId}";
		}

		public static string FightGroup(Guid fightId)
		{
			return $"Fight: {fightId}";
		}
	}
}
