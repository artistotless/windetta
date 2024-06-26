﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Windetta.Main.Infrastructure.Sagas;

#nullable disable

namespace Windetta.Main.Infrastructure.Data.Migrations
{
    [DbContext(typeof(SagasDbContext))]
    [Migration("20240327142443_AddedReadyToConnectStateProperty")]
    partial class AddedReadyToConnectStateProperty
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Windetta.Main.Infrastructure.Sagas.MatchFlow", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("char(36)");

                    b.Property<ulong>("BetAmount")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("BetCurrencyId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<sbyte>("CurrentState")
                        .HasColumnType("TINYINT");

                    b.Property<string>("Endpoint")
                        .HasColumnType("VARCHAR(42)")
                        .UseCollation("latin1_general_ci");

                    b.Property<Guid>("GameId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Players")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Properties")
                        .HasColumnType("longtext");

                    b.Property<int>("ReadyToConnectState")
                        .HasColumnType("int");

                    b.Property<string>("Tickets")
                        .HasColumnType("longtext")
                        .UseCollation("latin1_general_ci");

                    b.HasKey("CorrelationId");

                    b.HasIndex("CorrelationId");

                    b.ToTable("MatchFlow");
                });
#pragma warning restore 612, 618
        }
    }
}
