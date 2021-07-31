using System;

using FileReceiver.Dal.Entities.Enums;

using Microsoft.EntityFrameworkCore.Migrations;

namespace FileReceiver.Dal.Migrations
{
    public partial class Add_FileReceivingSession_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:FileReceivingSessionStateDb", "NewSession,FileSizeConstraintSet,FileNameConstraintSet,FileExtensionConstraintSet,SessionMaxFilesConstraint,SetFilesStorage,ActiveSession,EndedSession")
                .Annotation("Npgsql:Enum:FileStorageTypeDb", "None,Mega,GoogleDrive")
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:SessionEndReasonDb", "EndedByOwner,FilesLimitReached,StoppedDueError")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile,FileReceivingSessionCreating")
                .OldAnnotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .OldAnnotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .OldAnnotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile");

            migrationBuilder.CreateTable(
                name: "FileReceivingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SessionState = table.Column<FileReceivingSessionStateDb>(type: "\"FileReceivingSessionStateDb\"", nullable: false),
                    FilesReceived = table.Column<int>(type: "integer", nullable: false),
                    MaxFiles = table.Column<int>(type: "integer", nullable: false),
                    Storage = table.Column<FileStorageTypeDb>(type: "\"FileStorageTypeDb\"", nullable: true),
                    Constrains = table.Column<string>(type: "text", nullable: true),
                    CreateData = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SessionEndReason = table.Column<SessionEndReasonDb>(type: "\"SessionEndReasonDb\"", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileReceivingSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileReceivingSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileReceivingSessions_UserId",
                table: "FileReceivingSessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileReceivingSessions");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile")
                .OldAnnotation("Npgsql:Enum:FileReceivingSessionStateDb", "NewSession,FileSizeConstraintSet,FileNameConstraintSet,FileExtensionConstraintSet,SessionMaxFilesConstraint,SetFilesStorage,ActiveSession,EndedSession")
                .OldAnnotation("Npgsql:Enum:FileStorageTypeDb", "None,Mega,GoogleDrive")
                .OldAnnotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .OldAnnotation("Npgsql:Enum:SessionEndReasonDb", "EndedByOwner,FilesLimitReached,StoppedDueError")
                .OldAnnotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .OldAnnotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile,FileReceivingSessionCreating");
        }
    }
}
