using Gladiator.Data;
using Gladiator.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Gladiator.Models
{
	public class HighScore
	{
		// Get the highscores for fighters
		public static IList<FighterHighscoreDTO> GetFighterHighscores(GladiatorContext dbContext)
		{
			// Sort list with most wins at top, add positions
			IList<FighterHighscoreDTO> highscores = (from UFH in UnorderedFightersHighscore(dbContext) select UFH)
				.ToList()
				.OrderByDescending(wins => wins.FightsWon)
				// Add placement in list
				.Select((wo, pos) => new FighterHighscoreDTO
				{
					FighterId = wo.FighterId,
					Name = wo.Name,
					UserName = wo.UserName,
					UserId = wo.UserId,
					FightsWon = wo.FightsWon,
					FightsTotal = wo.FightsTotal,
					Position = pos + 1 // pos starts at 0, we want to start at 1
				})
				.ToList();
			return highscores;
		}

		// Get the highscores for Users
		public static IList<UserHighscoreDTO> GetUsersHighscores(GladiatorContext dbContext)
		{
			var unorderdUserHs =
				from wu in
		
					from users in dbContext.UserGladiator
					join UHF in UnorderedFightersHighscore(dbContext) on users.Id equals UHF.UserId into hs
					from UHFJoined in hs.DefaultIfEmpty()
					where users.Id > 1
					group users by new { UserId = users.Id, UserName = users.Name, UHFJoined.FightsWon, UHFJoined.FightsTotal } into grp
					select new UserHighscoreDTO
					{
						UserId = grp.Key.UserId,
						Name = grp.Key.UserName,
						FightsWon = grp.Sum(u => grp.Key.FightsWon),
						FightsTotal = grp.Sum(u => grp.Key.FightsTotal)
					}


				// Sum the users fighters to get user highscore
				group wu by new { wu.UserId, wu.Name } into grp
				select new UserHighscoreDTO
				{
					UserId = grp.Key.UserId,
					Name = grp.Key.Name,
					FightsWon = grp.Sum(g => g.FightsWon),
					FightsTotal = grp.Sum(g => g.FightsTotal)
				};






			var highscores =
				unorderdUserHs
				.ToList()
				.OrderByDescending(wins => wins.FightsWon)
				.Select((wo, pos) => new UserHighscoreDTO
				{
					Position = pos + 1, // We want our positions to start on  1, not 0 as pos does
					UserId = wo.UserId,
					Name = wo.Name,
					FightsWon = wo.FightsWon,
					FightsTotal = wo.FightsTotal,
				})
				.ToList();



			return highscores;

		}

		// To be able to resuse the Queryable as most of the code is the same for both user and fighters
		private static IQueryable<FighterHighscoreDTO> UnorderedFightersHighscore(GladiatorContext dbContext)
		{
			// Only get results from fights that has ended (WinnerFighterId should not be set anyway but better be safe)
			var endedFights =
				from fight in dbContext.Fights
				where fight.HasEnded
				select fight;

			// We do not want fighter owned by admin (UserId 1) or that are computeropponents
			var normalGladiators =
				from gladiator in dbContext.Gladiators
				where gladiator.UserId > 1 && !gladiator.IsOpponent
				select gladiator;


			// Find out how many wins the different fighters have
			var fightWinsCount =
				from gladiator in normalGladiators
				join fight in endedFights on (int?)gladiator.Id equals fight.WinnerFighterId into winner
				from fightJoined in winner.DefaultIfEmpty()
				group fightJoined by
					new { GladiatorId = gladiator.Id }
					into grp
				select new
				{
					FighterId = grp.Key.GladiatorId,
					FightsWon = grp.Sum(g => (int?)g.WinnerFighterId == grp.Key.GladiatorId ? 1 : 0),
				};


			var subCountTotalChallenger =
				// Count how many fights as a Challenger
				from gladiator in normalGladiators
				join fight in endedFights on gladiator.Id equals fight.ChallengerId into challenger
				from fightJoined in challenger.DefaultIfEmpty()
				group fightJoined by new { gID = gladiator.Id } into grp
				// Count how many games as an adversary the fighter has played
				select new
				{
					Id = grp.Key.gID,
					Count = grp.Sum(g => g.ChallengerId == grp.Key.gID ? 1 : 0)
				};

			var subCountTotalAdversary =
				// Count how man fights as an Adversary 
				from gladiator in normalGladiators
				join fight in endedFights on gladiator.Id equals fight.AdversaryId into adversary
				from fightJoined in adversary.DefaultIfEmpty()
				group fightJoined by new { gID = gladiator.Id } into grp
				// Count how many games as adversary the fighter has played
				select new
				{
					Id = grp.Key.gID,
					Count = grp.Sum(g => g.AdversaryId == grp.Key.gID ? 1 : 0)
				};

			// UNION ALL the both counts.
			var subCountTotal = subCountTotalAdversary.Concat(subCountTotalChallenger);

			// Sum the both counts up
			var fightCount =
					from subCount in subCountTotal
						// Sum fights both as challanger and adversary
					group subCount by new { subCount.Id } into subCountGrp
					select new
					{
						subCountGrp.Key.Id,
						Count = subCountGrp.Sum(g => g.Count)
					};


			// Combine the list of how many fights won with how many fights played
			var allCounts =
				from fwc in fightWinsCount
				join fc in fightCount on fwc.FighterId equals fc.Id
				group fwc by new { fwc.FighterId, fwc.FightsWon, fc.Count } into grp
				select new
				{
					grp.Key.FighterId,
					grp.Key.FightsWon,
					FightsTotal = grp.Sum(g => grp.Key.Count)
				};

			// And in userinfo to the list
			var unorderedHs =
				from gladiator in normalGladiators
				join user in dbContext.UserGladiator on gladiator.UserId equals user.Id
				join count in allCounts on gladiator.Id equals count.FighterId into counts
				from countJoined in counts.DefaultIfEmpty()
				group gladiator by new
				{
					FighterId = gladiator.Id,
					gladiator.Name,
					gladiator.UserId,
					UserName = user.Name,
					countJoined.FightsWon,
					countJoined.FightsTotal
				} into grp
				select new FighterHighscoreDTO
				{
					FighterId = grp.Key.FighterId,
					Name = grp.Key.Name,
					UserId = grp.Key.UserId,
					UserName = grp.Key.UserName,
					FightsWon = grp.Sum(g => grp.Key.FightsWon),
					FightsTotal = grp.Sum(g => grp.Key.FightsTotal),
				};

			return unorderedHs;
		}

	}
}
