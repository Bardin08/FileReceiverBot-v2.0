﻿// <auto-generated />
using System;
using FileReceiver.Dal;
using FileReceiver.Dal.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FileReceiver.Dal.Migrations
{
    [DbContext(typeof(FileReceiverDbContext))]
    partial class FileReceiverDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresEnum(null, "FileReceivingSessionStateDb", new[] { "NewSession", "FileSizeConstraintSet", "FileNameConstraintSet", "FileExtensionConstraintSet", "SessionMaxFilesConstraint", "SetFilesStorage", "ActiveSession", "EndedSession" })
                .HasPostgresEnum(null, "FileStorageTypeDb", new[] { "None", "Mega", "GoogleDrive" })
                .HasPostgresEnum(null, "RegistrationStateDb", new[] { "NewUser", "FirstNameReceived", "LastNameReceived", "SecretWordReceived", "RegistrationComplete" })
                .HasPostgresEnum(null, "SessionEndReasonDb", new[] { "EndedByOwner", "FilesLimitReached", "StoppedDueError" })
                .HasPostgresEnum(null, "TransactionStateDb", new[] { "Active", "Failed", "Aborted", "Committed" })
                .HasPostgresEnum(null, "TransactionTypeDb", new[] { "Unknown", "Registration", "EditProfile", "FileReceivingSessionCreating" })
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("FileReceiver.Dal.Entities.FileReceivingSessionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Constrains")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreateData")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FilesReceived")
                        .HasColumnType("integer");

                    b.Property<int>("MaxFiles")
                        .HasColumnType("integer");

                    b.Property<SessionEndReasonDb?>("SessionEndReason")
                        .HasColumnType("\"SessionEndReasonDb\"");

                    b.Property<FileReceivingSessionStateDb>("SessionState")
                        .HasColumnType("\"FileReceivingSessionStateDb\"");

                    b.Property<FileStorageTypeDb?>("Storage")
                        .HasColumnType("\"FileStorageTypeDb\"");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FileReceivingSessions");
                });

            modelBuilder.Entity("FileReceiver.Dal.Entities.TransactionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("TransactionData")
                        .HasColumnType("text");

                    b.Property<TransactionStateDb>("TransactionState")
                        .HasColumnType("\"TransactionStateDb\"");

                    b.Property<TransactionTypeDb>("TransactionType")
                        .HasColumnType("\"TransactionTypeDb\"");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("FileReceiver.Dal.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("RegistrationEndTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("RegistrationStartTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<RegistrationStateDb>("RegistrationState")
                        .HasColumnType("\"RegistrationStateDb\"");

                    b.Property<string>("SecretWordHash")
                        .HasColumnType("text");

                    b.Property<string>("TelegramTag")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FileReceiver.Dal.Entities.FileReceivingSessionEntity", b =>
                {
                    b.HasOne("FileReceiver.Dal.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FileReceiver.Dal.Entities.TransactionEntity", b =>
                {
                    b.HasOne("FileReceiver.Dal.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
