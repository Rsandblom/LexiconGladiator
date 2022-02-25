using Gladiator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Gladiator.Data
{
	public static class ModelBuilderExtension
    {

        public static void Seed(this ModelBuilder modelBuilder)
        {

            // Seed an administrator

            string roleIdAdmin = "334d04ff-98cd-4ffb-ad18-492a4ffc572c";
            string userIdAdmin = "2860a746-0879-412d-aadf-5893e2edfd00";

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = roleIdAdmin,
                Name = "Admin",
                NormalizedName = "ADMIN"

            });

            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();

            modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = userIdAdmin,
                Email = "admin@mail.com",
                NormalizedEmail = "ADMIN@MAIL.COM",
                UserName = "admin@mail.com",
                NormalizedUserName = "ADMIN@MAIL.COM",
                PasswordHash = hasher.HashPassword(null, "adminpass"),
                FirstName = "Admin",
                LastName = "Nimda"
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roleIdAdmin,
                UserId = userIdAdmin
            });

         // Seed Player 1

         string roleIdPlayer = "4e11c257-3fe9-46c2-ae90-8166b7a2aac2";
            string userIdPlayer = "da3f716e-dc6a-4988-b43e-f35f2ed7ea94";

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = roleIdPlayer,
                Name = "Player",
                NormalizedName = "PLAYER"

            });

            PasswordHasher<ApplicationUser> hasher2 = new PasswordHasher<ApplicationUser>();

            modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = userIdPlayer,
                Email = "player@mail.com",
                NormalizedEmail = "PLAYER@MAIL.COM",
                UserName = "player@mail.com",
                NormalizedUserName = "PLAYER@MAIL.COM",
                PasswordHash = hasher2.HashPassword(null, "playerpass"),
                FirstName = "Player1",
                LastName = "P1"
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roleIdPlayer,
                UserId = userIdPlayer
            });

            // Seed Player 2
        
            string userIdPlayer2 = "66d374f7-8e54-417e-9800-57a5a686f4aa";

            PasswordHasher<ApplicationUser> hasher3 = new PasswordHasher<ApplicationUser>();

            modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = userIdPlayer2,
                Email = "player2@mail.com",
                NormalizedEmail = "PLAYER2@MAIL.COM",
                UserName = "player2@mail.com",
                NormalizedUserName = "PLAYER2@MAIL.COM",
                PasswordHash = hasher3.HashPassword(null, "player2pass"),
                FirstName = "Player2",
                LastName = "P2"
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roleIdPlayer,
                UserId = userIdPlayer2
            });

            // ----- Add ---- Users to  admin and registered players
            // Admin
            modelBuilder.Entity<User>().HasData(new User { Id = 1, Name = "Admin", Gold = 0, ApplicationUserId = userIdAdmin });
            // Player 1
            modelBuilder.Entity<User>().HasData(new User { Id = 2, Name = "Numero uno", Gold = 100, ApplicationUserId = userIdPlayer});
            // Player 2
            modelBuilder.Entity<User>().HasData(new User { Id = 3, Name = "Dos Desperado", Gold = 100, ApplicationUserId = userIdPlayer2 });

            // -------- Add Modifier Creator -----------
            modelBuilder.Entity<Fighter>().HasData(new Fighter {Id = 1, Name = "Modifier Creator", Str = 1, Hp = 10, Xp = 0, Def = 0 , IsDeleted = false, IsOpponent = true, 
                HeadModifierId = 13, BodyModifierId = 14, RightHandModifierId = 15, LeftHandModifierId = 16, UserId = 1});

            // ---------- Add Modifiers ------------
            // Add Head type modifers 
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 1, Name ="Leather cap", Str =  0, Hp =  0, Xp = 0, Def = 1, Description = "Hardened leather cap", Price = 10, TypeOfGear = GearType.Head, FighterId = 1});
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 2, Name = "Bicycle helmet", Str = 0, Hp = 0, Xp = 0, Def = 2, Description = "Plastic and styrofoam", Price = 20, TypeOfGear = GearType.Head,  FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 3, Name = "Steel helmet", Str = 0, Hp = 0, Xp = 0, Def = 3, Description = "Shiny metal headpiece", Price = 30, TypeOfGear = GearType.Head, FighterId = 1 });

            // Add Body modifiers
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 4, Name = "Cotton Shirt", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Plain cotton shirt", Price = 10, TypeOfGear = GearType.Body, FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 5, Name = "Leather armor", Str = 0, Hp = 0, Xp = 0, Def = 2, Description = "Armor made of leather", Price = 20, TypeOfGear = GearType.Body, FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 6, Name = "Metal armor", Str = 10, Hp = 0, Xp = 0, Def = 3, Description = "Armor made of shiny metal", Price = 30, TypeOfGear = GearType.Body, FighterId = 1 });

            // Add Right hand modifiers
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 7, Name = "Wooden stick", Str = 1, Hp = 0, Xp = 0, Def = 0, Description = "Simple stick of wood", Price = 10, TypeOfGear = GearType.RightHand, FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 8, Name = "Short metal sword", Str = 2, Hp = 0, Xp = 0, Def = 0, Description = "Short standard metal sword", Price = 20, TypeOfGear = GearType.RightHand, FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 9, Name = "Long metal sword", Str = 3, Hp = 0, Xp = 0, Def = 0, Description = "Long metal sword", Price = 30, TypeOfGear = GearType.RightHand, FighterId = 1 });
            // Add Left hand modifiers

            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 10, Name = "Wooden shield", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Shield made of wood", Price = 10, TypeOfGear = GearType.LeftHand, FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 11, Name = "Metal shield", Str = 0, Hp = 0, Xp = 0, Def = 2, Description = "Shield made of metal", Price = 20, TypeOfGear = GearType.LeftHand, FighterId = 1 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 12, Name = "Large metal shield", Str = 0, Hp = 0, Xp = 0, Def = 3, Description = "Large shield made of wood", Price = 1, TypeOfGear = GearType.LeftHand, FighterId = 1 });


            // -------- Add Opponents -----------
            modelBuilder.Entity<Fighter>().HasData(new Fighter {Id = 2, Name = "Anna", Str = 1, Hp = 10, Xp = 0, Def = 0 , IsDeleted = false, IsOpponent = true, 
                HeadModifierId = 13, BodyModifierId = 14, RightHandModifierId = 15, LeftHandModifierId = 16, UserId = 1});

            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 13, Name = "Leather cap", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Hardened leather cap", Price = 10, TypeOfGear = GearType.Head, FighterId = 2 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 14, Name = "Cotton Shirt", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Plain cotton shirt", Price = 10, TypeOfGear = GearType.Body, FighterId = 2 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 15, Name = "Wooden stick", Str = 1, Hp = 0, Xp = 0, Def = 0, Description = "Simple stick of wood", Price = 10, TypeOfGear = GearType.RightHand, FighterId = 2 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 16, Name = "Wooden shield", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Shield made of wood", Price = 10, TypeOfGear = GearType.LeftHand, FighterId = 2 });


            // ------- Add Standard Gladiators --------
            modelBuilder.Entity<Fighter>().HasData(new Fighter {Id = 3, Name = "Std Gladiator 1", Str = 1, Hp = 10, Xp = 0, Def = 1 , IsDeleted = false, IsOpponent = false, 
                HeadModifierId = 17, BodyModifierId = 18, RightHandModifierId = 19, LeftHandModifierId = 20, UserId = 1});

            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 17, Name = "Leather cap", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Hardened leather cap", Price = 10, TypeOfGear = GearType.Head, FighterId = 3 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 18, Name = "Cotton Shirt", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Plain cotton shirt", Price = 10, TypeOfGear = GearType.Body, FighterId = 3 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 19, Name = "Wooden stick", Str = 1, Hp = 0, Xp = 0, Def = 0, Description = "Simple stick of wood", Price = 10, TypeOfGear = GearType.RightHand, FighterId = 3 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 20, Name = "Wooden shield", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Shield made of wood", Price = 10, TypeOfGear = GearType.LeftHand, FighterId = 3 });

            // ------- Add Player 1 Gladiators --------
            modelBuilder.Entity<Fighter>().HasData(new Fighter {Id = 4, Name = "P1 Gladiator 1", Str = 1, Hp = 10, Xp = 0, Def = 1 , IsDeleted = false, IsOpponent = false, 
                HeadModifierId = 21, BodyModifierId = 22, RightHandModifierId = 23, LeftHandModifierId = 24, UserId = 2});

            // --------
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 21, Name = "Leather cap", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Hardened leather cap", Price = 10, TypeOfGear = GearType.Head, FighterId = 4 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 22, Name = "Cotton Shirt", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Plain cotton shirt", Price = 10, TypeOfGear = GearType.Body, FighterId = 4 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 23, Name = "Wooden stick", Str = 1, Hp = 0, Xp = 0, Def = 0, Description = "Simple stick of wood", Price = 10, TypeOfGear = GearType.RightHand, FighterId = 4 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 24, Name = "Wooden shield", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Shield made of wood", Price = 10, TypeOfGear = GearType.LeftHand, FighterId = 4 });

         
            // ------- Add Player 2 Gladiators --------
            modelBuilder.Entity<Fighter>().HasData(new Fighter {Id = 5, Name = "P2 Gladiator 1", Str = 1, Hp = 10, Xp = 0, Def = 1 , IsDeleted = false, IsOpponent = false, 
                HeadModifierId = 25, BodyModifierId = 26, RightHandModifierId = 27, LeftHandModifierId = 28, UserId = 3});

            // --------
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 25, Name = "Leather cap", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Hardened leather cap", Price = 10, TypeOfGear = GearType.Head, FighterId = 5 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 26, Name = "Cotton Shirt", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Plain cotton shirt", Price = 10, TypeOfGear = GearType.Body, FighterId = 5 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 27, Name = "Wooden stick", Str = 1, Hp = 0, Xp = 0, Def = 0, Description = "Simple stick of wood", Price = 10, TypeOfGear = GearType.RightHand, FighterId = 5 });
            modelBuilder.Entity<Modifiers>().HasData(new Modifiers { Id = 28, Name = "Wooden shield", Str = 0, Hp = 0, Xp = 0, Def = 1, Description = "Shield made of wood", Price = 10, TypeOfGear = GearType.LeftHand, FighterId = 5 });






            // ------ Add Arenas -------
            modelBuilder.Entity<Arena>().HasData(new { Id = new Guid("341B68D0-6F19-4260-B056-2433F1AB49AB"), Name = "The Cave", Difficulty = 10 });
            modelBuilder.Entity<Arena>().HasData(new { Id = new Guid("7813A8BF-BA83-4D93-B0DE-4ED7F7C11C81"), Name = "The Rock", Difficulty = 10 });
            modelBuilder.Entity<Arena>().HasData(new { Id = new Guid("8EAFA150-D6D4-4909-8391-13C6C55F84D6"), Name = "The Firepit", Difficulty = 10 });
            modelBuilder.Entity<Arena>().HasData(new { Id = new Guid("5E22F9CC-175F-41B8-8C21-13D18FE28606"), Name = "The Dragons den", Difficulty = 10 });


      }


   }
}
