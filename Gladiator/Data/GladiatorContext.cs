using Gladiator.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gladiator.Data
{
	public class GladiatorContext: IdentityDbContext<ApplicationUser>
	{
		public GladiatorContext(DbContextOptions<GladiatorContext> options)
			 : base(options)
		{
		}

		public override DbSet<ApplicationUser> Users { get; set; }
		public DbSet<Fighter> Gladiators { get; set; }
		public DbSet<User> UserGladiator { get; set; }
		public DbSet<Modifiers> Modifiers { get; set; }

		public DbSet<Arena> Arenas { get; set; }
		public DbSet<Fight> Fights { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUser>()
				.HasOne(p => p.Player)
				.WithOne(u => u.ApplicationUser)
				.HasForeignKey<User>(u => u.ApplicationUserId);

			builder.Entity<Fight>()
				.HasOne(f => f.FightingArena)
				.WithMany(a => a.Fights)
				.HasForeignKey(f => f.ArenaId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Entity<Fight>()
				.HasOne(f => f.Challenger)
				.WithMany(f => f.ChallengerFights)
				.HasForeignKey(f => f.ChallengerId)
				.OnDelete(DeleteBehavior.NoAction)
				.IsRequired();

			builder.Entity<Fight>()
				.HasOne(f => f.Adversary)
				.WithMany(f => f.AdversaryFights)
				.HasForeignKey(f => f.AdversaryId)
				.OnDelete(DeleteBehavior.NoAction)
				.IsRequired();

			builder.Entity<Fight>()
				.HasOne(f => f.WinnerFighter)
				.WithMany(f => f.WinnerFights)
				.HasForeignKey(f => f.WinnerFighterId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Seed();
		}
	}
}
