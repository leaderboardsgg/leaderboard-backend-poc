﻿// <auto-generated />
using System;
using LeaderboardBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LeaderboardBackend.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20230624090236_Users_AddRoleAndDropAbout")]
    partial class UsersAddRoleAndDropAbout
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("LeaderboardId")
                        .HasColumnType("bigint")
                        .HasColumnName("leaderboard_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("PlayersMax")
                        .HasColumnType("integer")
                        .HasColumnName("players_max");

                    b.Property<int>("PlayersMin")
                        .HasColumnType("integer")
                        .HasColumnName("players_min");

                    b.Property<string>("Rules")
                        .HasColumnType("text")
                        .HasColumnName("rules");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.HasIndex("LeaderboardId")
                        .HasDatabaseName("ix_categories_leaderboard_id");

                    b.ToTable("categories", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Leaderboard", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Rules")
                        .HasColumnType("text")
                        .HasColumnName("rules");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.HasKey("Id")
                        .HasName("pk_leaderboards");

                    b.ToTable("leaderboards", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Run", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<LocalDate>("PlayedOn")
                        .HasColumnType("date")
                        .HasColumnName("played_on");

                    b.Property<Instant>("SubmittedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("submitted_at");

                    b.HasKey("Id")
                        .HasName("pk_runs");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_runs_category_id");

                    b.ToTable("runs", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<byte>("Role")
                        .HasColumnType("smallint")
                        .HasColumnName("role");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("ix_users_username");

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("421bb896-1990-48c6-8b0c-d69f56d6746a"),
                            Email = "omega@star.com",
                            Password = "$2a$11$tNvA94WqpJ.O7S7D6lVMn.E/UxcFYztl3BkcnBj/hgE8PY/8nCRQe",
                            Role = (byte)3,
                            Username = "Galactus"
                        });
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Category", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.Leaderboard", "Leaderboard")
                        .WithMany("Categories")
                        .HasForeignKey("LeaderboardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_categories_leaderboards_leaderboard_id");

                    b.Navigation("Leaderboard");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Run", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_runs_categories_category_id");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Leaderboard", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
