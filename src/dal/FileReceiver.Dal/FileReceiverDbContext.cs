using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Npgsql;
using Npgsql.NameTranslation;

namespace FileReceiver.Dal
{
    public class FileReceiverDbContext : DbContext
    {
        public FileReceiverDbContext(DbContextOptions options) : base(options)
        {
            RegisterPgTypes();
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<FileReceivingSessionEntity> FileReceivingSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum(nameof(TransactionTypeDb), typeof(TransactionTypeDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(TransactionStateDb), typeof(TransactionStateDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(RegistrationStateDb), typeof(RegistrationStateDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(SessionEndReasonDb), typeof(SessionEndReasonDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(FileReceivingSessionStateDb), typeof(FileReceivingSessionStateDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(FileStorageTypeDb), typeof(FileStorageTypeDb).GetEnumNames());
        }

        private static void RegisterPgTypes()
        {
            NpgsqlConnection.GlobalTypeMapper
                .MapEnum<TransactionTypeDb>(
                    nameof(TransactionTypeDb), new NpgsqlNullNameTranslator())
                .MapEnum<TransactionStateDb>(
                    nameof(TransactionStateDb), new NpgsqlNullNameTranslator())
                .MapEnum<RegistrationStateDb>(
                    nameof(RegistrationStateDb), new NpgsqlNullNameTranslator())
                .MapEnum<SessionEndReasonDb>(
                    nameof(SessionEndReasonDb), new NpgsqlNullNameTranslator())
                .MapEnum<FileReceivingSessionStateDb>(
                    nameof(FileReceivingSessionStateDb), new NpgsqlNullNameTranslator())
                .MapEnum<FileStorageTypeDb>(
                    nameof(FileStorageTypeDb), new NpgsqlNullNameTranslator());
        }
    }
}
