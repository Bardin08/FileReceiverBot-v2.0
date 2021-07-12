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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum(nameof(TransactionTypeDb), typeof(TransactionTypeDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(TransactionStateDb), typeof(TransactionStateDb).GetEnumNames());
            modelBuilder.HasPostgresEnum(nameof(RegistrationStateDb), typeof(RegistrationStateDb).GetEnumNames());
        }

        private static void RegisterPgTypes()
        {
            NpgsqlConnection.GlobalTypeMapper
                .MapEnum<TransactionTypeDb>(
                    nameof(TransactionTypeDb), new NpgsqlNullNameTranslator())
                .MapEnum<TransactionStateDb>(
                    nameof(TransactionStateDb), new NpgsqlNullNameTranslator())
                .MapEnum<RegistrationStateDb>(
                    nameof(RegistrationStateDb), new NpgsqlNullNameTranslator());
        }
    }
}
