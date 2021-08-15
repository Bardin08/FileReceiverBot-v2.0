using Microsoft.EntityFrameworkCore.Migrations;

namespace FileReceiver.Dal.Migrations
{
    public partial class Update_Transaction_Types_Enum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:FileReceivingSessionStateDb", "NewSession,FileSizeConstraintSet,FileNameConstraintSet,FileExtensionConstraintSet,SessionMaxFilesConstraint,SetFilesStorage,ActiveSession,EndedSession")
                .Annotation("Npgsql:Enum:FileStorageTypeDb", "None,Mega,GoogleDrive")
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:SessionEndReasonDb", "EndedByOwner,FilesLimitReached,StoppedDueError")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile,FileReceivingSessionCreating,FileSending")
                .OldAnnotation("Npgsql:Enum:FileReceivingSessionStateDb", "NewSession,FileSizeConstraintSet,FileNameConstraintSet,FileExtensionConstraintSet,SessionMaxFilesConstraint,SetFilesStorage,ActiveSession,EndedSession")
                .OldAnnotation("Npgsql:Enum:FileStorageTypeDb", "None,Mega,GoogleDrive")
                .OldAnnotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .OldAnnotation("Npgsql:Enum:SessionEndReasonDb", "EndedByOwner,FilesLimitReached,StoppedDueError")
                .OldAnnotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .OldAnnotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile,FileReceivingSessionCreating");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:FileReceivingSessionStateDb", "NewSession,FileSizeConstraintSet,FileNameConstraintSet,FileExtensionConstraintSet,SessionMaxFilesConstraint,SetFilesStorage,ActiveSession,EndedSession")
                .Annotation("Npgsql:Enum:FileStorageTypeDb", "None,Mega,GoogleDrive")
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:SessionEndReasonDb", "EndedByOwner,FilesLimitReached,StoppedDueError")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile,FileReceivingSessionCreating")
                .OldAnnotation("Npgsql:Enum:FileReceivingSessionStateDb", "NewSession,FileSizeConstraintSet,FileNameConstraintSet,FileExtensionConstraintSet,SessionMaxFilesConstraint,SetFilesStorage,ActiveSession,EndedSession")
                .OldAnnotation("Npgsql:Enum:FileStorageTypeDb", "None,Mega,GoogleDrive")
                .OldAnnotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .OldAnnotation("Npgsql:Enum:SessionEndReasonDb", "EndedByOwner,FilesLimitReached,StoppedDueError")
                .OldAnnotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .OldAnnotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile,FileReceivingSessionCreating,FileSending");
        }
    }
}
