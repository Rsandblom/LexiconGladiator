using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gladiator.Migrations
{
    public partial class NewApproach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGladiator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Gold = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGladiator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGladiator_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Gladiators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Str = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Xp = table.Column<int>(nullable: false),
                    Def = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsOpponent = table.Column<bool>(nullable: false),
                    HeadModifierId = table.Column<int>(nullable: false),
                    BodyModifierId = table.Column<int>(nullable: false),
                    LeftHandModifierId = table.Column<int>(nullable: false),
                    RightHandModifierId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gladiators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gladiators_UserGladiator_UserId",
                        column: x => x.UserId,
                        principalTable: "UserGladiator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modifiers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Str = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Xp = table.Column<int>(nullable: false),
                    Def = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    TypeOfGear = table.Column<int>(nullable: false),
                    FighterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modifiers_Gladiators_FighterId",
                        column: x => x.FighterId,
                        principalTable: "Gladiators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6abf93df-183e-4824-bf1a-db8ff0e80f9b", "9adc215f-6e94-4c30-91fe-631cb26606d6", "Admin", "ADMIN" },
                    { "c9ca3c72-ac4e-4752-a27a-f6c17d75d9b1", "8b0008b1-b961-433d-98af-b4474ba8308e", "Player", "PLAYER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "2860a746-0879-412d-aadf-5893e2edfd00", 0, "673591e8-5293-4e46-a4dd-369350b6c927", "admin@mail.com", false, "Admin", "Nimda", false, null, "ADMIN@MAIL.COM", "ADMIN@MAIL.COM", "AQAAAAEAACcQAAAAEDnINHVgXQEVF6hb0pzG2TlnujfTxHmPgQUgvDQmNgE3FEA9726YlyTGSuSrFrlVkw==", null, false, "895c1a86-1c41-4e04-b605-1404be203eb0", false, "admin@mail.com" },
                    { "da3f716e-dc6a-4988-b43e-f35f2ed7ea94", 0, "d42f80c6-09ec-4c3b-90a7-901d1851956e", "player@mail.com", false, "Player1", "P1", false, null, "PLAYER@MAIL.COM", "PLAYER@MAIL.COM", "AQAAAAEAACcQAAAAEIUDLi4aa4k1W5toOGd7C9ZESLRqyA1KzJxQQGHaOay1xdfwD8wfqpSCroL009ZADQ==", null, false, "e9c30a6a-22bb-4b2a-bc05-ddb0b8c7e945", false, "player@mail.com" },
                    { "66d374f7 - 8e54 - 417e-9800 - 57a5a686f4aa", 0, "9cd86b89-6c90-4c1d-9ecc-ea5b4ff64756", "player2@mail.com", false, "Player2", "P2", false, null, "PLAYER2@MAIL.COM", "PLAYER2@MAIL.COM", "AQAAAAEAACcQAAAAENhlrsaJUhmjs3QVQnfKJnwJE0n0EC10eRoAoBvscB1zKtx2XqooBAgPJ7ZsRYJ7/g==", null, false, "b8e52a67-ebf4-4384-b7c6-c0b38eaa7e15", false, "player2@mail.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "2860a746-0879-412d-aadf-5893e2edfd00", "6abf93df-183e-4824-bf1a-db8ff0e80f9b" },
                    { "da3f716e-dc6a-4988-b43e-f35f2ed7ea94", "c9ca3c72-ac4e-4752-a27a-f6c17d75d9b1" },
                    { "66d374f7 - 8e54 - 417e-9800 - 57a5a686f4aa", "c9ca3c72-ac4e-4752-a27a-f6c17d75d9b1" }
                });

            migrationBuilder.InsertData(
                table: "UserGladiator",
                columns: new[] { "Id", "ApplicationUserId", "Gold", "Name" },
                values: new object[,]
                {
                    { 1, "2860a746-0879-412d-aadf-5893e2edfd00", 0, "Admin" },
                    { 2, "da3f716e-dc6a-4988-b43e-f35f2ed7ea94", 100, "Numero uno" },
                    { 3, "66d374f7 - 8e54 - 417e-9800 - 57a5a686f4aa", 100, "Dos Desperado" }
                });

            migrationBuilder.InsertData(
                table: "Gladiators",
                columns: new[] { "Id", "BodyModifierId", "Def", "HeadModifierId", "Hp", "IsDeleted", "IsOpponent", "LeftHandModifierId", "Name", "RightHandModifierId", "Str", "UserId", "Xp" },
                values: new object[,]
                {
                    { 1, 14, 0, 13, 10, false, true, 16, "Modifier Creator", 15, 1, 1, 0 },
                    { 2, 14, 0, 13, 10, false, true, 16, "Opponent 1", 15, 1, 1, 0 },
                    { 3, 18, 1, 17, 10, false, false, 20, "Std Gladiator 1", 19, 1, 1, 0 },
                    { 4, 21, 1, 20, 10, false, false, 24, "P1 Gladiator 1", 23, 1, 2, 0 }
                });

            migrationBuilder.InsertData(
                table: "Modifiers",
                columns: new[] { "Id", "Def", "Description", "FighterId", "Hp", "Name", "Price", "Str", "TypeOfGear", "Xp" },
                values: new object[,]
                {
                    { 1, 1, "Hardened leather cap", 1, 0, "Leather cap", 10, 0, 0, 0 },
                    { 22, 1, "Plain cotton shirt", 4, 0, "Cotton Shirt", 10, 0, 1, 0 },
                    { 21, 1, "Hardened leather cap", 4, 0, "Leather cap", 10, 0, 0, 0 },
                    { 20, 1, "Shield made of wood", 3, 0, "Wooden shield", 10, 0, 2, 0 },
                    { 19, 0, "Simple stick of wood", 3, 0, "Wooden stick", 10, 1, 3, 0 },
                    { 18, 1, "Plain cotton shirt", 3, 0, "Cotton Shirt", 10, 0, 1, 0 },
                    { 17, 1, "Hardened leather cap", 3, 0, "Leather cap", 10, 0, 0, 0 },
                    { 16, 1, "Shield made of wood", 2, 0, "Wooden shield", 10, 0, 2, 0 },
                    { 15, 0, "Simple stick of wood", 2, 0, "Wooden stick", 10, 1, 3, 0 },
                    { 14, 1, "Plain cotton shirt", 2, 0, "Cotton Shirt", 10, 0, 1, 0 },
                    { 13, 1, "Hardened leather cap", 2, 0, "Leather cap", 10, 0, 0, 0 },
                    { 12, 3, "Large shield made of wood", 1, 0, "Large metal shield", 1, 0, 2, 0 },
                    { 11, 2, "Shield made of metal", 1, 0, "Metal shield", 20, 0, 2, 0 },
                    { 10, 1, "Shield made of wood", 1, 0, "Wooden shield", 10, 0, 2, 0 },
                    { 9, 0, "Long metal sword", 1, 0, "Long metal sword", 30, 3, 3, 0 },
                    { 8, 0, "Short standard metal sword", 1, 0, "Short metal sword", 20, 2, 3, 0 },
                    { 7, 0, "Simple stick of wood", 1, 0, "Wooden stick", 10, 1, 3, 0 },
                    { 6, 3, "Armor made of shiny metal", 1, 0, "Metal armor", 30, 10, 1, 0 },
                    { 5, 2, "Armor made of leather", 1, 0, "Leather armor", 20, 0, 1, 0 },
                    { 4, 1, "Plain cotton shirt", 1, 0, "Cotton Shirt", 10, 0, 1, 0 },
                    { 3, 3, "Shiny metal headpiece", 1, 0, "Steel helmet", 30, 0, 0, 0 },
                    { 2, 2, "Plastic and styrofoam", 1, 0, "Bicycle helmet", 20, 0, 0, 0 },
                    { 23, 0, "Simple stick of wood", 4, 0, "Wooden stick", 10, 1, 3, 0 },
                    { 24, 1, "Shield made of wood", 4, 0, "Wooden shield", 10, 0, 2, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Gladiators_UserId",
                table: "Gladiators",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Modifiers_FighterId",
                table: "Modifiers",
                column: "FighterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGladiator_ApplicationUserId",
                table: "UserGladiator",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Modifiers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Gladiators");

            migrationBuilder.DropTable(
                name: "UserGladiator");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
