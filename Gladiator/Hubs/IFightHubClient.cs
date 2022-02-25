using Gladiator.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gladiator.Models
{
	public interface IFightHubClient
	{
		Task Error(string errorMessage);
		Task UserFightIdList(IList<Guid> fightIds);

		Task FightChallenge(Guid fightId, FightChallengeDTO fightChallangeData);
		Task FightChallengeAccepted(Guid fightId, int yourFighterId);

		Task JoinedFight(Guid fightId, int fighterId);
		Task FightStarted(Guid fightId);
		Task FightEnded(Guid fightId);
		Task FightCancelled(Guid fightId);
		
		Task OpponentLeftFight(Guid fightId);


		Task AttackAllowed(Guid fightId, bool isAttackAllowed);
		Task Attacked(Guid fightId);


		Task RoundData(Guid fightId, FightRoundDTO fightRoundData);
		Task FightData(Guid fightId, FightDTO fightData);
		Task FightResults(Guid fightId, FightResultDTO fightResultData);

		Task FighterHighscores(IList<FighterHighscoreDTO> highscores);
		Task UserHighscores(IList<UserHighscoreDTO> highscores);




	}
}
