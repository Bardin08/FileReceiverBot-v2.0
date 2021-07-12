using System;

using FileReceiver.Dal.Entities.Enums;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileReceiver.Dal.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TelegramTag = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    SecretWordHash = table.Column<string>(type: "text", nullable: true),
                    RegistrationState = table.Column<RegistrationStateDb>(type: "\"RegistrationStateDb\"", nullable: false),
                    RegistrationStartTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RegistrationEndTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionType = table.Column<TransactionTypeDb>(type: "\"TransactionTypeDb\"", nullable: false),
                    TransactionData = table.Column<string>(type: "text", nullable: true),
                    TransactionState = table.Column<TransactionStateDb>(type: "\"TransactionStateDb\"", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
