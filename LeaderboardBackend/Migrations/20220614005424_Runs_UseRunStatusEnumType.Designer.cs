﻿// <auto-generated />
using System;
using LeaderboardBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LeaderboardBackend.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20220614005424_Runs_UseRunStatusEnumType")]
    partial class Runs_UseRunStatusEnumType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "metric_type", new[] { "int", "float", "interval" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "run_status", new[] { "created", "submitted", "pending", "approved", "rejected" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Ban", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("BannedUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("banned_user_id");

                    b.Property<Guid?>("BanningUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("banning_user_id");

                    b.Property<long?>("LeaderboardId")
                        .HasColumnType("bigint")
                        .HasColumnName("leaderboard_id");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time");

                    b.HasKey("Id")
                        .HasName("pk_bans");

                    b.HasIndex("BannedUserId")
                        .HasDatabaseName("ix_bans_banned_user_id");

                    b.HasIndex("BanningUserId")
                        .HasDatabaseName("ix_bans_banning_user_id");

                    b.HasIndex("LeaderboardId")
                        .HasDatabaseName("ix_bans_leaderboard_id");

                    b.ToTable("bans", (string)null);
                });

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

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Judgement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool?>("Approved")
                        .HasColumnType("boolean")
                        .HasColumnName("approved");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("ModId")
                        .HasColumnType("uuid")
                        .HasColumnName("mod_id");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("note");

                    b.Property<Guid>("RunId")
                        .HasColumnType("uuid")
                        .HasColumnName("run_id");

                    b.HasKey("Id")
                        .HasName("pk_judgements");

                    b.HasIndex("ModId")
                        .HasDatabaseName("ix_judgements_mod_id");

                    b.HasIndex("RunId")
                        .HasDatabaseName("ix_judgements_run_id");

                    b.ToTable("judgements", (string)null);
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

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Metric", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Max")
                        .HasColumnType("integer")
                        .HasColumnName("max");

                    b.Property<int>("Min")
                        .HasColumnType("integer")
                        .HasColumnName("min");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<MetricType>("Type")
                        .HasColumnType("metric_type")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_metrics");

                    b.ToTable("metrics", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Modship", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("LeaderboardId")
                        .HasColumnType("bigint")
                        .HasColumnName("leaderboard_id");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_modships");

                    b.HasIndex("LeaderboardId")
                        .HasDatabaseName("ix_modships_leaderboard_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_modships_user_id");

                    b.ToTable("modships", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Participation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment");

                    b.Property<Guid>("RunId")
                        .HasColumnType("uuid")
                        .HasColumnName("run_id");

                    b.Property<Guid>("RunnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("runner_id");

                    b.Property<string>("Vod")
                        .HasColumnType("text")
                        .HasColumnName("vod");

                    b.HasKey("Id")
                        .HasName("pk_participations");

                    b.HasIndex("RunId")
                        .HasDatabaseName("ix_participations_run_id");

                    b.HasIndex("RunnerId")
                        .HasDatabaseName("ix_participations_runner_id");

                    b.ToTable("participations", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Run", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Played")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("played");

                    b.Property<RunStatus>("Status")
                        .HasColumnType("run_status")
                        .HasColumnName("status");

                    b.Property<DateTime>("Submitted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("submitted");

                    b.HasKey("Id")
                        .HasName("pk_runs");

                    b.ToTable("runs", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("About")
                        .HasColumnType("text")
                        .HasColumnName("about");

                    b.Property<bool>("Admin")
                        .HasColumnType("boolean")
                        .HasColumnName("admin");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Ban", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.User", "BannedUser")
                        .WithMany("BansReceived")
                        .HasForeignKey("BannedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_bans_users_banned_user_id");

                    b.HasOne("LeaderboardBackend.Models.Entities.User", "BanningUser")
                        .WithMany("BansGiven")
                        .HasForeignKey("BanningUserId")
                        .HasConstraintName("fk_bans_users_banning_user_id");

                    b.HasOne("LeaderboardBackend.Models.Entities.Leaderboard", "Leaderboard")
                        .WithMany("Bans")
                        .HasForeignKey("LeaderboardId")
                        .HasConstraintName("fk_bans_leaderboards_leaderboard_id");

                    b.Navigation("BannedUser");

                    b.Navigation("BanningUser");

                    b.Navigation("Leaderboard");
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

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Judgement", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.User", "Mod")
                        .WithMany()
                        .HasForeignKey("ModId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_judgements_users_mod_id");

                    b.HasOne("LeaderboardBackend.Models.Entities.Run", "Run")
                        .WithMany("Judgements")
                        .HasForeignKey("RunId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_judgements_runs_run_id");

                    b.Navigation("Mod");

                    b.Navigation("Run");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Modship", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.Leaderboard", "Leaderboard")
                        .WithMany("Modships")
                        .HasForeignKey("LeaderboardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_modships_leaderboards_leaderboard_id");

                    b.HasOne("LeaderboardBackend.Models.Entities.User", "User")
                        .WithMany("Modships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_modships_users_user_id");

                    b.Navigation("Leaderboard");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Participation", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.Run", "Run")
                        .WithMany("Participations")
                        .HasForeignKey("RunId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_participations_runs_run_id");

                    b.HasOne("LeaderboardBackend.Models.Entities.User", "Runner")
                        .WithMany("Participations")
                        .HasForeignKey("RunnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_participations_users_runner_id");

                    b.Navigation("Run");

                    b.Navigation("Runner");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Leaderboard", b =>
                {
                    b.Navigation("Bans");

                    b.Navigation("Categories");

                    b.Navigation("Modships");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Run", b =>
                {
                    b.Navigation("Judgements");

                    b.Navigation("Participations");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.User", b =>
                {
                    b.Navigation("BansGiven");

                    b.Navigation("BansReceived");

                    b.Navigation("Modships");

                    b.Navigation("Participations");
                });
#pragma warning restore 612, 618
        }
    }
}
