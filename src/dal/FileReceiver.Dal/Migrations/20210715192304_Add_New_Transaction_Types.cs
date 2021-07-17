using Microsoft.EntityFrameworkCore.Migrations;

namespace FileReceiver.Dal.Migrations
{
    public partial class Add_New_Transaction_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile")
                .OldAnnotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .OldAnnotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .OldAnnotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .Annotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .Annotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration")
                .OldAnnotation("Npgsql:Enum:RegistrationStateDb", "NewUser,FirstNameReceived,LastNameReceived,SecretWordReceived,RegistrationComplete")
                .OldAnnotation("Npgsql:Enum:TransactionStateDb", "Active,Failed,Aborted,Committed")
                .OldAnnotation("Npgsql:Enum:TransactionTypeDb", "Unknown,Registration,EditProfile");
        }
    }
}
