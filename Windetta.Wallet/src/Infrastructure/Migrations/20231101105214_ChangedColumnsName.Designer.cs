﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Windetta.Wallet.Infrastructure.Data;

#nullable disable

namespace Windetta.Wallet.Migrations
{
    [DbContext(typeof(WalletDbContext))]
    [Migration("20231101105214_ChangedColumnsName")]
    partial class ChangedColumnsName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Windetta.Wallet.Domain.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("VARCHAR(40)");

                    b.Property<long>("Amount")
                        .HasColumnType("BIGINT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("DATETIME(6)");

                    b.Property<sbyte>("Type")
                        .HasColumnType("TINYINT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Windetta.Wallet.Domain.UserWallet", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("CHAR(36)");

                    b.Property<long>("Balance")
                        .HasColumnType("BIGINT");

                    b.Property<long>("HeldBalance")
                        .HasColumnType("BIGINT");

                    b.HasKey("UserId");

                    b.ToTable("Wallets");
                });
#pragma warning restore 612, 618
        }
    }
}
