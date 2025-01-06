// <auto-generated />
using System;
using LeaderboardBackend.Models;
using LeaderboardBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LeaderboardBackend.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "run_type", new[] { "time", "score" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "sort_direction", new[] { "ascending", "descending" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_role", new[] { "registered", "confirmed", "administrator", "banned" });
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "citext");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.AccountConfirmation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Instant>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<Instant?>("UsedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("used_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_account_confirmations");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_account_confirmations_user_id");

                    b.ToTable("account_confirmations", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.AccountRecovery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Instant>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<Instant?>("UsedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("used_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_account_recoveries");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_account_recoveries_user_id");

                    b.ToTable("account_recoveries", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Instant?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Info")
                        .HasColumnType("text")
                        .HasColumnName("info");

                    b.Property<long>("LeaderboardId")
                        .HasColumnType("bigint")
                        .HasColumnName("leaderboard_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("character varying(80)")
                        .HasColumnName("slug");

                    b.Property<SortDirection>("SortDirection")
                        .HasColumnType("sort_direction")
                        .HasColumnName("sort_direction");

                    b.Property<RunType>("Type")
                        .HasColumnType("run_type")
                        .HasColumnName("type");

                    b.Property<Instant?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_categories");

                    b.HasIndex("LeaderboardId", "Slug")
                        .IsUnique()
                        .HasDatabaseName("ix_categories_leaderboard_id_slug");

                    b.ToTable("categories", null, t =>
                        {
                            t.HasCheckConstraint("CK_categories_name_MinLength", "LENGTH(name) >= 1");

                            t.HasCheckConstraint("CK_categories_slug_MinLength", "LENGTH(slug) >= 2");

                            t.HasCheckConstraint("CK_categories_slug_RegularExpression", "slug ~ '^[a-zA-Z0-9\\-_]*$'");
                        });
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Leaderboard", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Instant?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Info")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("")
                        .HasColumnName("info");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("character varying(80)")
                        .HasColumnName("slug");

                    b.Property<Instant?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_leaderboards");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasDatabaseName("ix_leaderboards_slug")
                        .HasFilter("deleted_at IS NULL");

                    b.ToTable("leaderboards", null, t =>
                        {
                            t.HasCheckConstraint("CK_leaderboards_name_MinLength", "LENGTH(name) >= 1");

                            t.HasCheckConstraint("CK_leaderboards_slug_MinLength", "LENGTH(slug) >= 2");

                            t.HasCheckConstraint("CK_leaderboards_slug_RegularExpression", "slug ~ '^[a-zA-Z0-9\\-_]*$'");
                        });
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

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Instant?>("DeletedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Info")
                        .HasColumnType("text")
                        .HasColumnName("info");

                    b.Property<LocalDate>("PlayedOn")
                        .HasColumnType("date")
                        .HasColumnName("played_on");

                    b.Property<long>("TimeOrScore")
                        .HasColumnType("bigint")
                        .HasColumnName("time_or_score");

                    b.Property<Instant?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_runs");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_runs_category_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_runs_user_id");

                    b.ToTable("runs", (string)null);
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("citext")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<UserRole>("Role")
                        .HasColumnType("user_role")
                        .HasColumnName("role");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("citext")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("ix_users_username");

                    b.ToTable("users", null, t =>
                        {
                            t.HasCheckConstraint("CK_users_email_EmailAddress", "email ~ '^[^@]+@[^@]+$'");

                            t.HasCheckConstraint("CK_users_password_MinLength", "LENGTH(password) >= 1");

                            t.HasCheckConstraint("CK_users_username_MinLength", "LENGTH(username) >= 2");

                            t.HasCheckConstraint("CK_users_username_RegularExpression", "username ~ '^[a-zA-Z0-9]([-_'']?[a-zA-Z0-9])+$'");
                        });
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.AccountConfirmation", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_confirmations_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.AccountRecovery", b =>
                {
                    b.HasOne("LeaderboardBackend.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_account_recoveries_users_user_id");

                    b.Navigation("User");
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

                    b.HasOne("LeaderboardBackend.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_runs_users_user_id");

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LeaderboardBackend.Models.Entities.Leaderboard", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
