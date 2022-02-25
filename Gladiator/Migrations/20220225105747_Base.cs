using Microsoft.EntityFrameworkCore.Migrations;

namespace Gladiator.Migrations
{
    public partial class Base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "334d04ff-98cd-4ffb-ad18-492a4ffc572c",
                column: "ConcurrencyStamp",
                value: "31282d01-9fd0-4acb-b60c-630e2828c553");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e11c257-3fe9-46c2-ae90-8166b7a2aac2",
                column: "ConcurrencyStamp",
                value: "02ac4649-514e-4b60-b7e7-1281f865cc74");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2860a746-0879-412d-aadf-5893e2edfd00",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "946fc208-9371-4ce4-8d30-689f1a761d0b", "AQAAAAEAACcQAAAAENBX471QFVlEV3zGLEbqKAeIPHSYO5TYln9DZAP4Uj2vMU5L165lz/vfQuPY1/5ImQ==", "38f9a461-90a1-43c7-96cc-dc49a4365d89" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "66d374f7-8e54-417e-9800-57a5a686f4aa",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e29e77f-0933-4d4c-ae6e-c69589895e9e", "AQAAAAEAACcQAAAAEErMiUGlDQiqp/CgE/HjKc4DN550ioA2pFsIDyPHzd2yqy4e64HiUPdye6Wb5QpeYg==", "8bdca0ff-36a6-4808-826a-1fa847f389f5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "da3f716e-dc6a-4988-b43e-f35f2ed7ea94",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a526da40-82e5-44da-b273-86526efb508e", "AQAAAAEAACcQAAAAEBB0kig2XUaXzwYo9UVvb75HzUrT0OiyDG7ZUXYExiWkLYHEoaFiZP3TyRoyKZCuOg==", "89318e64-c739-4304-86f5-f65ee444e0b6" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "334d04ff-98cd-4ffb-ad18-492a4ffc572c",
                column: "ConcurrencyStamp",
                value: "f28eb14c-62c0-47f5-8125-ab70d5c0af33");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4e11c257-3fe9-46c2-ae90-8166b7a2aac2",
                column: "ConcurrencyStamp",
                value: "03302d51-fa8e-4aec-951a-ae2fc5fa896c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2860a746-0879-412d-aadf-5893e2edfd00",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c4d33019-61ef-4ef7-8cfb-67d20e4cc41e", "AQAAAAEAACcQAAAAEOY9vgGq69HRQcYWkxqkkY7la6qGxA1hckNRCb3j2PZSbuV918TcxBSKfQKnPMFjGA==", "60e629de-5363-44ad-9944-e05a7794bee2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "66d374f7-8e54-417e-9800-57a5a686f4aa",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cfff122d-539d-4869-a9d5-12237104d302", "AQAAAAEAACcQAAAAEAkNRpKQ32G1d1wgycuQQx9bsEyasrmt6vkFfqIZDFxnvgMgowXWg2MXqEiXDuFjAA==", "309cc760-6419-4c58-9b9f-ff73b3a01438" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "da3f716e-dc6a-4988-b43e-f35f2ed7ea94",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5937022c-dfe8-4d8a-a624-f60ae36a9a57", "AQAAAAEAACcQAAAAEDPVGuGEBOf15YQOnxwwbv3NgJWeNaB3zh3W0BN4VzpB6kosWbDD2DZZLulxvC1EfQ==", "6b0555f7-ff45-4182-9872-2877623ae740" });
        }
    }
}
