using Gladiator.Data;
using Gladiator.DTOs;
using Gladiator.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.Models
{
	public class Fight
	{
		// Keep track of the right fight
		[Key]
		public Guid Id { get; private set; }
		public Guid ArenaId { get; private set; }
		public Arena FightingArena { get; private set; }
		public bool HasStarted { get; private set; } = false;
		public bool HasEnded { get; private set; } = false;
		public bool IsCancelled { get; private set; } = false;

		// ID:s to keep track which fighter is which
		public Fighter Challenger { get; private set; }
		public int ChallengerId { get; private set; }
		public int ChallengerCurrentHp
		{
			get => _challengerCurrentHp;
			private set => _challengerCurrentHp = value < 0 ? 0 : value;
		}
		public bool IsChallengerAllowedToAttack { get; private set; } = false;
		public bool HasChallengerJoinedFight { get; private set; } = false;

		public Fighter Adversary { get; private set; }
		public int AdversaryId { get; private set; }
		public int AdversaryCurrentHp
		{
			get => _adversaryCurrentHp;
			private set => _adversaryCurrentHp = value < 0 ? 0 : value;
		}
		public bool IsAdversaryAllowedToAttack { get; private set; } = false;
		public bool HasAdversaryJoinedFight { get; private set; } = false;


		public int? WinnerFighterId { get; private set; }
		public Fighter WinnerFighter { get; private set; }
		public int Round { get; private set; } = 0;
		public int XpChange { get; private set; } = 0;
		public int GoldWon { get; private set; } = 0;

		private int _adversaryCurrentHp = 0;
		private int _challengerCurrentHp = 0;

		private Fight() { }
		public Fight(Arena arena, Fighter fighterChallenger, Fighter fighterAdversary)
		{
			// Initiations
			HasEnded = false;
			FightingArena = arena;
			ArenaId = arena.Id;

			Challenger = fighterChallenger;
			ChallengerId = fighterChallenger.Id;
			ChallengerCurrentHp = fighterChallenger.Hp;
			IsChallengerAllowedToAttack = false;
			HasChallengerJoinedFight = false;

			Adversary = fighterAdversary;
			AdversaryId = fighterAdversary.Id;
			AdversaryCurrentHp = fighterAdversary.Hp;
			IsAdversaryAllowedToAttack = false;
			HasAdversaryJoinedFight = false;
		}

		public static bool Challange(Arena arena, Fighter fighterChallenger, Fighter fighterAdversary, out Fight newFight,
		IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{

			newFight = null;

			if(fighterChallenger.UserId == fighterAdversary.UserId)
			{
				// You are not allowed to challange yourself
				hubContext.Clients.Group(HubGroup.FighterGroup(fighterChallenger.Id)).Error("Fighters have the same owner");
				return false;
			}

			newFight = new Fight(arena, fighterChallenger, fighterAdversary);

			// Save new fight to database, and set the Id
			dbContext.Fights.Add(newFight);
			dbContext.SaveChanges();

			if(fighterAdversary.IsOpponent)
			{
				// The adversarty joins the fight right away
				// if they are played by the computer.
				newFight.HasAdversaryJoinedFight = true;
				dbContext.Fights.Update(newFight);
				dbContext.SaveChanges();
			}
			else
			{
				newFight.Invite2ndUserToFight(hubContext);
			}
			return true;
		}




		// Used after a reconnect et.c.
		public static bool TryJoinFight(Guid fightId, int fighterId, IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{

			// Check that it is a fight that either not started or active.
			if(dbContext.Fights.Any(f => f.Id == fightId && !f.HasEnded && !f.IsCancelled))
			{
				// Check that the fighter is in the fight
				if(dbContext.Fights.Any(f => f.Id == fightId && ( f.AdversaryId == fighterId || f.ChallengerId == fighterId)))
				{

					{
						// Update that the fighter has joined the fight
						var fightUpdate = dbContext.Fights.Where(f => f.Id == fightId).FirstOrDefault();
						if(dbContext.Fights.Any(f => f.Id == fightId &&  f.AdversaryId == fighterId))
						{
							fightUpdate.Id = fightId;
							fightUpdate.HasAdversaryJoinedFight = true;
						}
						else
						{
							fightUpdate.Id = fightId;
							fightUpdate.HasChallengerJoinedFight = true;
						}
						dbContext.Fights.Update(fightUpdate);
						dbContext.SaveChanges();
					}

					hubContext.Clients.Group(HubGroup.FighterGroup(fighterId)).JoinedFight(fightId, fighterId);

					// Both players want to play, let us get started, as the fight has not started yet
					if (dbContext.Fights.Any(f => f.HasChallengerJoinedFight && f.HasAdversaryJoinedFight && !f.HasStarted && f.Id == fightId))
					{
						// Select starter user, set fight to HasStarted = true, send info
						StartFight(fightId, hubContext, dbContext);
					}
					else
					{
						// The game has either already started or both fighters are not redy yet
						// Send the ready player some fight information
						Fight fight = dbContext.Fights.Include(f => f.FightingArena).Include(f => f.Challenger)
							.Include(f => f.Adversary).SingleOrDefault(f => f.Id == fightId);
						if(fight is null)
						{
							hubContext.Clients.Group(HubGroup.FighterGroup(fighterId)).Error("Could not load fight");
							return false;
						}
						fight.SendCurrentFightDataToFighter(fighterId, hubContext, dbContext);
					}

					return true;
				}
				else
				{
					// Fight exists but fighter does not
					hubContext.Clients.Group(HubGroup.FighterGroup(fighterId)).Error("Fighter is not in fight!");
					return false;
				}
			}
			else
			{
				// Fight not found
				hubContext.Clients.Group(HubGroup.FighterGroup(fighterId)).Error("Fight does not exist or is not active!");
				return false;
			}
		}

		public async Task LeaveFight(int fighterId, IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{
			if(IsFighterInFight(fighterId, dbContext))
			{
				if(!HasStarted)
				{
					await hubContext.Clients.Group(HubGroup.FightGroup(Id)).FightCancelled(Id);
					IsCancelled = true;
					HasEnded = true;
					UpdateFightInDb(this, dbContext);
					return;
				}
				else if(!HasEnded)
				{
					XpChange = CalculateXpChange() * 2;
					GoldWon = CalculateGoldWon();
					WinnerFighterId = GetIdForOpponentToFighterId(fighterId, dbContext);

					int opponentId = fighterId == ChallengerId ? ChallengerId : AdversaryId;

					await hubContext.Clients.Group(HubGroup.FighterGroup(opponentId)).OpponentLeftFight(Id);
					await EndGameAsync(hubContext, dbContext);
					UpdateFightInDb(this, dbContext);
				}
			}
			else
			{
				await hubContext.Clients.Group(HubGroup.FighterGroup(fighterId)).Error($"Fighter {fighterId} is not in Fight!");
			}

		}


		public async Task Attack(int attackerId, IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{
			if(!IsFighterInFight(attackerId, dbContext))
			{
				throw new ArgumentException("Attacker could not be found!");
			}

			int defenderId = GetIdForOpponentToFighterId(attackerId, dbContext);

			if(!GetIsAllowedToAttackById(attackerId))
			{
				await hubContext.Clients.Group(HubGroup.FighterGroup(attackerId)).Error("Attack not allowed");
				return;
			}

			// No attacking allowed for now
			SetIsAllowedToAttackById(attackerId, false, dbContext);
			SetIsAllowedToAttackById(defenderId, false, dbContext);
			UpdateFightInDb(this, dbContext);
			await hubContext.Clients.Group(HubGroup.FightGroup(Id)).AttackAllowed(Id, false);

			await hubContext.Clients.Group(HubGroup.FighterGroup(defenderId)).Attacked(Id);

			// We have fighters!
			Round++;

			// To get values of stats with mods applied

			Random rnd = new Random();

			Fighter attacker = GetFighterFromDbById(attackerId, dbContext);
			Fighter defender = GetFighterFromDbById(defenderId, dbContext);

			// Fight
			int attackStrenght = rnd.Next(
				attacker.CalculatedStrenght / 2,
				attacker.CalculatedStrenght + 1) + rnd.Next(1, 21) + rnd.Next(1, 21);

			int defenseStrenght = rnd.Next(
				defender.CalculatedDefence / 2,
				defender.CalculatedDefence + 1) + rnd.Next(1, 21) + rnd.Next(1, 21);

			int defenderHpLoss = 0;

			int roundWinner = -1;   // Negative == tie

			string desc = "";

			if(attackStrenght > defenseStrenght)
			{
				for(int roll = 0; roll < FightingArena.Difficulty; roll++)
				{
					desc += offStr(attacker.Name, defender.Name) + defStr(attacker.Name, defender.Name);
					roundWinner = attackerId;
					defenderHpLoss += rnd.Next(1, 7);
				}
			}
			else if(attackStrenght < defenseStrenght)
			{
				for(int roll = 0; roll < FightingArena.Difficulty; roll++)
				{
					desc += offStr(attacker.Name, defender.Name) + defStr(attacker.Name, defender.Name);
					roundWinner = defenderId;
				}
			}

			AddToCurrentHpById(defenderId, -defenderHpLoss, dbContext); // Note add negative
			UpdateFightInDb(this, dbContext);

			var attackerFighterData = new FightRoundDTO.FighterData
			{
				FighterId = attackerId,
				IsAttacker = true,
				FightStrenght = attackStrenght,
				HpLoss = 0,
				CurrentHp = GetCurrentHpById(attackerId),
				CurrentHpPercent = (GetCurrentHpById(attackerId) * 100) / attacker.Hp,
				StartHp = attacker.Hp
			};

			var defenderFighterData = new FightRoundDTO.FighterData
			{
				FighterId = defenderId,
				IsAttacker = false,
				FightStrenght = defenseStrenght,
				HpLoss = defenderHpLoss,
				CurrentHp = GetCurrentHpById(defenderId),
				CurrentHpPercent = (GetCurrentHpById(defenderId) * 100) / defender.Hp,
				StartHp = defender.Hp
			};

			var attackerRoundData = new FightRoundDTO
			{
				FightId = Id,
				RoundNo = Round,
				SuccessfulDefence = roundWinner == defenderId,
				You = attackerFighterData,
				Opponent = defenderFighterData,
				RoundDescription = desc
			};

			var defenderRoundData = new FightRoundDTO
			{
				FightId = Id,
				RoundNo = Round,
				SuccessfulDefence = roundWinner == defenderId,
				You = defenderFighterData,
				Opponent = attackerFighterData,
				RoundDescription = desc	
			};

			// Fight takes some time ...
			await Task.Delay(new Random().Next(1500, 10001));

			await hubContext.Clients.Group(HubGroup.FighterGroup(attackerId)).RoundData(Id, attackerRoundData);
			await hubContext.Clients.Group(HubGroup.FighterGroup(defenderId)).RoundData(Id, defenderRoundData);

			if(GetCurrentHpById(attackerId) > 0 && GetCurrentHpById(defenderId) > 0)
			{
				// No one has died, continue fight
				SetIsAllowedToAttackById(attackerId, false, dbContext);
				SetIsAllowedToAttackById(defenderId, true, dbContext);
				UpdateFightInDb(this, dbContext);

				await hubContext.Clients.Group(HubGroup.FighterGroup(attackerId))
					.AttackAllowed(Id, GetIsAllowedToAttackById(attackerId));
				await hubContext.Clients.Group(HubGroup.FighterGroup(defenderId))
					.AttackAllowed(Id, GetIsAllowedToAttackById(defenderId));

				return;
			}
			else
			{
				// Award XP and Gold et.c.

				if(GetCurrentHpById(attackerId) <= 0 && GetCurrentHpById(defenderId) <= 0)
				{
					await hubContext.Clients.Group(HubGroup.FightGroup(Id)).FightEnded(Id);
					await hubContext.Clients.Group(HubGroup.FightGroup(Id)).Error("Tie");
					// Both fighters died
					throw new ApplicationException("There cannot be a tie!");
				}


				if(GetCurrentHpById(attackerId) > 0)
				{
					// The attacker won as the defender died
					WinnerFighterId = attackerId;
				}
				else // if(CurrentHpFighterAdversary > 0)
				{
					// The defender won as the attacker died
					WinnerFighterId = defenderId;
				}


				GoldWon = CalculateGoldWon();
				XpChange = CalculateXpChange();

				await EndGameAsync(hubContext, dbContext);

				UpdateFightInDb(this, dbContext);
			}
		}


		public static Fight GetFightByIdFromDb(Guid fightId, GladiatorContext dbContext)
		{
			return dbContext.Fights
				.Include(f => f.Challenger)
				.Include(f => f.Adversary)
				.Include(f => f.FightingArena)
				.Include(f => f.WinnerFighter)
				.Where(f => f.Id == fightId)
				.FirstOrDefault();
		}

		public static List<Guid> GetUsersActiveFightIds(int userId, GladiatorContext dbContext)
		{
			var fights =
				from fight in dbContext.Fights
				where (fight.Challenger.UserId == userId
						|| fight.Adversary.UserId == userId)
						&& !fight.IsCancelled
						&& !fight.HasEnded
				select fight.Id;

			return fights.ToList();
		}

		public static IList<Fight> GetUsersNonAcceptedFights(int userId, GladiatorContext dbContext)
		{
			return (
				from fight in dbContext.Fights
				where fight.Adversary.UserId == userId && !fight.HasStarted && !fight.IsCancelled && !fight.HasEnded
				select fight
				).ToList();
		}

		public bool IsFighterInFight(int fighterId, GladiatorContext dbContext)
		{
			try
			{
				dbContext.Fights
				.Where(f => f.Id == Id && (f.ChallengerId == fighterId || f.AdversaryId == fighterId))
				.Single();

				return true;
			}
			catch(Exception)
			{
				return false;
			}
		}


		public Fighter GetFighterFromDbById(int fighterId, GladiatorContext dbContext)
		{
			Fighter fighter = dbContext.Gladiators
				.Where(g => g.Id == fighterId)
				.Include(g => g.Modifiers)
				.Include(g => g.User)
				.FirstOrDefault();

			if(fighter == null)
			{
				throw new ArgumentException("Invalid Fighter ID!");
			}

			fighter.SetModifiers();
			fighter.CalculateStatsWithPlacedModifiers();

			return fighter;
		}

		// Who is the other fighter?
		public int GetIdForOpponentToFighterId(int fighterId, GladiatorContext dbContext)
		{
			if(IsFighterInFight(fighterId, dbContext))
				return fighterId == ChallengerId ? AdversaryId : ChallengerId;

			throw new ArgumentException("Fighter not in fight!");
		}

		public void SendCurrentFightDataToFighter(int fighterId, IHubContext<FightHub,
			IFightHubClient> hubContext, GladiatorContext dbContext)
		{

			if(!IsFighterInFight(fighterId, dbContext))
			{
				throw new ArgumentException("Fighter does not exist");
			}

			// Who is who?
			int fighterIdOpponent = fighterId == ChallengerId ? AdversaryId : ChallengerId;

			// Prepare data to send to fighters
			var arenaData = new FightDTO.ArenaData
			{
				Id = FightingArena.Id,
				Name = FightingArena.Name,
				Difficulty = FightingArena.Difficulty
			};


			FightDTO.FighterData fighterData = GetFighterData(fighterId, dbContext);

			FightDTO.FighterData fighterDataOpponent = GetFighterData(fighterIdOpponent, dbContext);

			var fightData = new FightDTO
			{
				FightId = Id,
				Arena = arenaData,
				You = fighterData,
				Opponent = fighterDataOpponent,
			};

			// Send data to fighter
			_ = hubContext.Clients.Group(HubGroup.FighterGroup(fighterId)).FightData(Id, fightData);

			_ = hubContext.Clients.Group(HubGroup.FighterGroup(fighterId))
				.AttackAllowed(Id, GetIsAllowedToAttackById(fighterId));

		}


		public static bool UpdateFightInDb(Fight fight, GladiatorContext dbContext)
		{
			var fightUpdate = dbContext.Fights.Where(f => f.Id == fight.Id).FirstOrDefault();
			if(fightUpdate is null)
			{
				return false;
			}

			fightUpdate.Round = fight.Round;
			fightUpdate.WinnerFighterId = fight.WinnerFighterId;
			fightUpdate.XpChange = fight.XpChange;
			fightUpdate.GoldWon = fight.GoldWon;
			fightUpdate.HasStarted = fight.HasStarted;
			fightUpdate.HasEnded = fight.HasEnded;
			fightUpdate.IsCancelled = fight.IsCancelled;


			_ = dbContext.Fights.Update(fightUpdate);
			return dbContext.SaveChanges() > 0;
		}

		private FightDTO.FighterData GetFighterData(int fighterId, GladiatorContext dbContext)
		{
			Fighter fighter = GetFighterFromDbById(fighterId, dbContext);

			return new FightDTO.FighterData
			{
				FighterId = fighter.Id,
				UserId = fighter.UserId,
				Name = fighter.Name,
				BaseStrength = fighter.Str,
				Strength = fighter.CalculatedStrenght,
				BaseDefence = fighter.Def,
				Defence = fighter.CalculatedDefence,
				Xp = fighter.Xp,
				StartHp = fighter.Hp,
				CurrentHp = GetCurrentHpById(fighterId),
				CurrentHpPercent = fighter.Hp == 0 ? 0 : ((GetCurrentHpById(fighterId) * 100) / fighter.Hp),
				IsComputer = fighter.IsOpponent,
			};
		}

		private async Task EndGameAsync(IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{

			int userXpChange, adversaryXpChange;
			int userGoldWon, adversaryGoldWon;

			if(WinnerFighterId == ChallengerId)
			{
				// User won
				userXpChange = XpChange;
				adversaryXpChange = 0;
				userGoldWon = GoldWon;
				adversaryGoldWon = 0;
			}
			else
			{
				// Adversary won
				userXpChange = 0;
				adversaryXpChange = XpChange;
				userGoldWon = 0;
				adversaryGoldWon = GoldWon;
			}

			// Send result to players

			var challengerStats = new FightResultDTO.FighterStats
			{
				FighterId = ChallengerId,
				XpChange = userXpChange,
				GoldWon = userGoldWon,
			};

			var adversaryStats = new FightResultDTO.FighterStats
			{
				FighterId = AdversaryId,
				XpChange = adversaryXpChange,
				GoldWon = adversaryGoldWon,
			};

			FightResultDTO fightResults = new FightResultDTO
			{
				FightId = Id,
				AreanaId = FightingArena.Id,
				Difficulty = FightingArena.Difficulty,
				NoRounds = Round,
				ChallengerStats = challengerStats,
				AdversaryStats = adversaryStats,
			};

			HasEnded = true;
			await hubContext.Clients.Group(HubGroup.FightGroup(Id)).FightEnded(Id);
			await hubContext.Clients.Group(HubGroup.FightGroup(Id)).FightResults(Id, fightResults);

			bool winnerIsOpponent = WinnerFighterId == ChallengerId ? Challenger.IsOpponent : Adversary.IsOpponent;


			if(!winnerIsOpponent && (int)WinnerFighterId != 0)
			{
				var gladiator = GetFighterFromDbById((int)WinnerFighterId, dbContext);
				if(gladiator == null)
				{
					throw new Exception("Gladiator not found. Could not update");
				}
				gladiator.Xp += XpChange;

				// Give gold to winner user
				gladiator.User.Gold += GoldWon;

				dbContext.SaveChanges();
			}

			//bool loserIsOpponent = WinnerFighterId == ChallengerId ? Adversary.IsOpponent : Challenger.IsOpponent;

			//if(!loserIsOpponent)
			//{
			//	var gladiator = GetFighterFromDbById(GetIdForOpponentToFighterId((int)WinnerFighterId, dbContext), dbContext);
			//	if(gladiator == null)
			//	{
			//		throw new Exception("Gladiator not found. Could not update");
			//	}
			//	gladiator.Xp -= XpChange;

			//	dbContext.SaveChanges();
			//}

		}

		private int CalculateXpChange()
		{
			// TODO: Replace with something better -->
			return new Random().Next(FightingArena.Difficulty * 100, FightingArena.Difficulty * 200 +1);
		}

		private int CalculateGoldWon()
		{
			// TODO: Replace with something better -->
			return FightingArena.Difficulty * new Random().Next(1, FightingArena.Difficulty * 10);
		}

		public bool GetIsAllowedToAttackById(int fighterId)
		{
			if(fighterId == ChallengerId)
			{
				return IsChallengerAllowedToAttack;
			}
			else if(fighterId == AdversaryId)
			{
				return IsAdversaryAllowedToAttack;
			}

			throw new ArgumentException("Fighter not in fight!");
		}

		private bool SetIsAllowedToAttackById(int fighterId, bool isAllowedToAttack, GladiatorContext dbContext)
		{
			if(fighterId == ChallengerId)
			{
				IsChallengerAllowedToAttack = isAllowedToAttack;
				UpdateFightInDb(this, dbContext);
				return IsChallengerAllowedToAttack;
			}
			else if(fighterId == AdversaryId)
			{
				IsAdversaryAllowedToAttack = isAllowedToAttack;
				UpdateFightInDb(this, dbContext);
				return IsAdversaryAllowedToAttack;
			}

			throw new ArgumentException("Fighter not in fight!");
		}

		private int GetCurrentHpById(int fighterId)
		{
			if(fighterId == ChallengerId)
			{
				return ChallengerCurrentHp;
			}
			else if(fighterId == AdversaryId)
			{
				return AdversaryCurrentHp;
			}

			throw new ArgumentException("Fighter not in fight!");
		}

		private int SetCurrentHpById(int fighterId, int newHp, GladiatorContext dbContext)
		{
			if(fighterId == ChallengerId)
			{
				ChallengerCurrentHp = newHp;
				UpdateFightInDb(this, dbContext);
				return ChallengerCurrentHp;
			}
			else if(fighterId == AdversaryId)
			{
				AdversaryCurrentHp = newHp;
				UpdateFightInDb(this, dbContext);
				return AdversaryCurrentHp;
			}

			throw new ArgumentException("Fighter not in fight!");
		}

		private int AddToCurrentHpById(int fighterId, int toAdd, GladiatorContext dbContext)
		{
			if(fighterId == ChallengerId)
			{
				ChallengerCurrentHp += toAdd;
				UpdateFightInDb(this, dbContext);
				return ChallengerCurrentHp;
			}
			else if(fighterId == AdversaryId)
			{
				AdversaryCurrentHp += toAdd;
				UpdateFightInDb(this, dbContext);
				return ChallengerCurrentHp;
			}

			throw new ArgumentException("Fighter not in fight!");

		}

		private void Invite2ndUserToFight(IHubContext<FightHub, IFightHubClient> hubContext)
		{
			FightChallengeDTO fightChallangeData = new FightChallengeDTO
			{
				FightId = Id,
				ArenaId = FightingArena.Id,
				ArenaName = FightingArena.Name,
				ArenaDifficulty = FightingArena.Difficulty,
				Challenger = new FightChallengeDTO.FighterData
				{
					FighterId = Challenger.Id,
					FighterName = Challenger.Name,
					FighterUserName = Challenger.User.Name,
					FighterXp = Challenger.Xp,
					FighterHp = Challenger.Hp,
					FighterStrenght = Challenger.CalculatedStrenght,
					FighterDefence = Challenger.CalculatedDefence,
				},
				Adversary = new FightChallengeDTO.FighterData
				{
					FighterId = Adversary.Id,
					FighterName = Adversary.Name,
					FighterUserName = Adversary.User.Name,
					FighterXp = Adversary.Xp,
					FighterHp = Adversary.Hp,
					FighterStrenght = Adversary.CalculatedStrenght,
					FighterDefence = Adversary.CalculatedDefence,
				}
			};

			// Send Invitation to User
			hubContext.Clients.User(Adversary.User.ApplicationUserId).FightChallenge(Id, fightChallangeData);
		}

		private static bool StartFight(Guid fightId, IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{

			Fight fight = dbContext.Fights.Include(f=> f.FightingArena).SingleOrDefault (f => f.Id == fightId);
			if(fight is null)
			{
				return false;
			}
			// Select who starts
			if(new Random().Next(0, 2) == 1)
			{
				fight.IsChallengerAllowedToAttack = true;
				fight.IsAdversaryAllowedToAttack = false;
			}
			else
			{
				fight.IsChallengerAllowedToAttack = false;
				fight.IsAdversaryAllowedToAttack = true;
			}

			fight.HasStarted = true;
			dbContext.SaveChanges();
//			UpdateFightInDb(this, dbContext);

			fight.SendCurrentFightDataToFighter(fight.ChallengerId, hubContext, dbContext);
			fight.SendCurrentFightDataToFighter(fight.AdversaryId, hubContext, dbContext);

			hubContext.Clients.Group(HubGroup.FightGroup(fight.Id)).FightStarted(fight.Id);

			hubContext.Clients.Group(HubGroup.FighterGroup(fight.ChallengerId))
				.AttackAllowed(fight.Id, fight.IsChallengerAllowedToAttack);

			hubContext.Clients.Group(HubGroup.FighterGroup(fight.AdversaryId))
				.AttackAllowed(fight.Id, fight.IsAdversaryAllowedToAttack);

			return true;
		}

		private string defStr(string a, string d)
		{
			return a + GW(OT) + d + " with " +GW(DA) + "attack. ";
		}
		private string offStr (string a, string d)
		{
			return d + GW(DT) + a + "s " + GW(OA) + "attack. "; ;
		}

		private string GW(string[] str)
        {
			return str[new Random().Next(0, str.Length)];
        }

		private string[] OT = { " bruises ",
								" stubs " ,
								" hits " ,
								" nicks " ,
								" scratches " ,
								" pummels " ,
								" mauls " };
		private string[] DT = { " dogdes ",
								" parries ",
								" deflects ",
								" glances off " ,
								" ducks from " ,
								" eludes "  };
		private string[] DA = { "a lazy ",
								"a fine ",
								"a terrible ",
								"an amateurish " ,
								"an excellent " ,
								"a feeble " ,
								"an useless "  };
		private string[] OA = { "lazy ",
								"fine ",
								"terrible ",
								"amateurish " ,
								"excellent " ,
								"feeble " ,
								"useless "  };
	}

	
}
