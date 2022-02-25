using Gladiator.Data;
using Gladiator.DTOs;
using Gladiator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gladiator.Hubs
{
	[Authorize]
	public class FightHub: Hub<IFightHubClient>
	{
		private readonly IHubContext<FightHub, IFightHubClient> _hubContext;
		private readonly GladiatorContext _dbContext;

		public FightHub(IHubContext<FightHub, IFightHubClient> hubContext, GladiatorContext dbContext)
		{
			_hubContext = hubContext;
			_dbContext = dbContext;
		}

		public async Task Attack(Guid fightId, int attackerId)
		{
			Fight fight = Fight.GetFightByIdFromDb(fightId, _dbContext);
			if(!(fight is null) && fight.IsFighterInFight(attackerId, _dbContext) && IsFighterOwnedByUser(attackerId))
			{
				int defenderId;
				try
				{
					defenderId = fight.GetIdForOpponentToFighterId(attackerId, _dbContext);
				}
				catch
				{
					await Clients.Caller.Error("Attacker does not exist in fight!");
					return;
				}

				await fight.Attack(attackerId, _hubContext, _dbContext);

				if(!fight.HasEnded && IsFighterOwnedByComputer(defenderId))
				{
					// Attacks back after 1 - 5 second
					await Task.Delay(new Random().Next(0, 4001) + 1000);
					await fight.Attack(defenderId, _hubContext, _dbContext);
				}
			}
			else
			{
				await Clients.Caller.Error("Attacker does not exist in fight!");
				return;
			}
		}

		public async Task StartFight(int challengerFighterId, int adversaryFighterId, Guid arenaId)
		{

			if(IsFighterInDb(challengerFighterId) && IsFighterInDb(adversaryFighterId) && IsArenaInDb(arenaId))
			{
				await AddFighterToFighterGroupAsync(challengerFighterId);

				Arena arena = GetArenaFromDbById(arenaId);
				Fighter challenger = GetFighterFromDbById(challengerFighterId);
				Fighter adversary = GetFighterFromDbById(adversaryFighterId);

				if(Fight.Challange(arena, challenger, adversary, out Fight fight, _hubContext, _dbContext))
				{
					await JoinFight(fight.Id, challengerFighterId);

					if(_dbContext.Fights.Any(f => f.Id == fight.Id && f.HasStarted && f.Adversary.IsOpponent && f.IsAdversaryAllowedToAttack))
					{
						// The computer starts attacking this fight

						// Give things a chance to load for the user
						await Task.Delay(10000); 
						
						// The computer attacks!
						await fight.Attack(adversaryFighterId, _hubContext, _dbContext);
					}
				}
				else
				{
					await Clients.Caller.Error("Could not start fight!");
					await RemoveFighterFromGroupsAsync(fight.Id, challengerFighterId);
				}
			}

		}


		// Request to join fight, for example after a disconnect or reload
		public async Task JoinFight(Guid fightId, int fighterId)
		{
			Fight fight = Fight.GetFightByIdFromDb(fightId, _dbContext);

			// Fight exist, fighter is in fight and is owned by the user calling
			if(!(fight is null) && fight.IsFighterInFight(fighterId, _dbContext) && IsFighterOwnedByUser(fighterId))
			{
				// Have to add to groups before joining the fight to be able
				// to receive messages when joining fight
				await AddFighterToGroupsAsync(fightId, fighterId);

				await Task.Delay(1000);

				if(Fight.TryJoinFight(fightId, fighterId, _hubContext, _dbContext))
				{
					await Clients
						.Group(HubGroup.FighterGroup(fighterId))
						.JoinedFight(fightId, fighterId);
					fight.SendCurrentFightDataToFighter(fighterId, _hubContext, _dbContext);

					if(_dbContext.Fights.Any(f => f.Id == fight.Id && f.HasStarted && f.Adversary.IsOpponent && f.IsAdversaryAllowedToAttack))
					{
						// It's the computers turn next, and it needs some help. :-)
						await fight.Attack(fight.AdversaryId, _hubContext, _dbContext);
					}
					return;
				}
				else
				{
					// Remove from groups as fight could not be joined
					await RemoveFighterFromGroupsAsync(fightId, fighterId);
				}

			}
			await Clients.Caller.Error("DidNotJoinFight");
		}


		public async Task LeaveFight(Guid fightId, int fighterId)
		{
			Fight fight = Fight.GetFightByIdFromDb(fightId, _dbContext);
			if(!(fight is null) && fight.IsFighterInFight(fighterId, _dbContext))
			{
				await fight.LeaveFight(fighterId, _hubContext, _dbContext);
				await RemoveFighterFromGroupsAsync(fightId, fighterId);
			}
			else
			{
				await Clients.Caller.Error("Could not leave fight. Fight not found or fighter not in fight.");
			}
		}

		public async Task GetPresentFightData(Guid? fightId, int? fighterId)
		{
			fightId ??= Guid.Empty;
			fighterId ??= 0;
			if(fightId != Guid.Empty && fighterId > 1)
			{
				Fight fight = Fight.GetFightByIdFromDb((Guid)fightId, _dbContext);
				if(fight.IsFighterInFight((int)fighterId, _dbContext))
				{
					try
					{
						fight.SendCurrentFightDataToFighter((int)fighterId, _hubContext, _dbContext);
						return;
					}
					catch(ArgumentException)
					{
						await Clients.Caller.Error("Could Not Get Fight Data");
					}

				}
			}
			await Clients.Caller.Error("Arguments not valid. Could not get Fight Data");
		}

		public async Task GetUsersFights()
		{
			try
			{
				int userId = await _dbContext.UserGladiator
									.Where(ug => ug.ApplicationUserId == Context.UserIdentifier)
									.Select(ug => ug.Id)
									.SingleAsync();

				List<Guid> fightIds = Fight.GetUsersActiveFightIds(userId, _dbContext);

				await Clients.Caller.UserFightIdList(fightIds);
			}
			catch
			{
				await Clients.Caller.Error("User not found!");
			}
		}

		public async Task GetFighterHighscores()
		{
			try
			{
				await Clients.Caller.FighterHighscores(HighScore.GetFighterHighscores(_dbContext));
			}
			catch(Exception ex)
			{
				await Clients.Caller.Error(ex.ToString());
			}
		}


		public async Task GetUserHighscores()
		{
			try
			{
				await Clients.Caller.UserHighscores(HighScore.GetUsersHighscores(_dbContext));
			}
			catch(Exception ex)
			{
				await Clients.Caller.Error(ex.ToString());
			}
		}






		private async Task<bool> AddFighterToFighterGroupAsync(int fighterId)
		{
			if(IsFighterInDb(fighterId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, HubGroup.FighterGroup(fighterId));
				return true;
			}

			return false;

		}

		private async Task<bool> AddFighterToFightGroupAsync(Guid fightId, int fighterId)
		{
			Fight fight = Fight.GetFightByIdFromDb(fightId, _dbContext);

			if(IsFighterInDb(fighterId)
				&& !(fight is null)
				&& fight.IsFighterInFight(fighterId, _dbContext))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, HubGroup.FightGroup(fightId));
				return true;
			}

			return false;
		}

		// Add Fighter to fight and fighter groups
		private async Task<bool> AddFighterToGroupsAsync(Guid fightId, int fighterId)
		{
			Fight fight = Fight.GetFightByIdFromDb(fightId, _dbContext);

			if(IsFighterInDb(fighterId)
				&& !(fight is null)
				&& fight.IsFighterInFight(fighterId, _dbContext))
			{
				if(await AddFighterToFighterGroupAsync(fighterId))
				{
					if(await AddFighterToFightGroupAsync(fightId, fighterId))
					{
						return true;
					}
					else
					{
						await RemoveFighterFromGroupsAsync(fightId, fighterId);
					}
				}
			}
			return false;
		}

		// Remove Fighter from fight and fighter groups
		private async Task RemoveFighterFromGroupsAsync(Guid fightId, int fighterId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, HubGroup.FightGroup(fightId));

			if(IsFighterInDb(fighterId))
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, HubGroup.FighterGroup(fighterId));
		}


		private bool IsFighterInDb(int fighterId)
		{
			return _dbContext.Gladiators.Count(f => f.Id == fighterId) == 1;
		}

		private Fighter GetFighterFromDbById(int fighterId)
		{
			Fighter fighter = _dbContext.Gladiators
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

		private bool IsFighterOwnedByComputer(int fighterId)
		{
			var fighter = _dbContext.Gladiators.Find(fighterId);

			if(fighter is null)
				throw new Exception("Fighter does not exist!");

			return fighter.IsOpponent;
		}

		private bool IsFighterOwnedByUser(int fighterId)
		{
			return _dbContext.Gladiators
			.Include(g => g.User)
			.Where(g => g.Id == fighterId && g.User.ApplicationUserId == Context.UserIdentifier)
			.Count() == 1;
		}


		private bool IsArenaInDb(Guid arenaId)
		{
			return _dbContext.Arenas.Count(a => a.Id == arenaId) == 1;
		}

		private Arena GetArenaFromDbById(Guid arenaId)
		{
			return _dbContext.Arenas.Find(arenaId);
		}
	}


}
