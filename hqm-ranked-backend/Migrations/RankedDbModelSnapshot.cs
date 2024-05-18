﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using hqm_ranked_backend.Models.DbModels;

#nullable disable

namespace hqm_ranked_backend.Migrations
{
    [DbContext(typeof(RankedDb))]
    partial class RankedDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Avatar", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Thumbnail")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Avatars");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Bans", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("BannedPlayerId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Days")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BannedPlayerId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Elo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uuid");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("SeasonId");

                    b.ToTable("Elos");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.EventType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MaxX")
                        .HasColumnType("integer");

                    b.Property<int>("MaxY")
                        .HasColumnType("integer");

                    b.Property<int>("MinX")
                        .HasColumnType("integer");

                    b.Property<int>("MinY")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("EventTypes");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.EventWinners", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("PlayerId");

                    b.ToTable("EventWinners");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Events", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("EventTypeId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("X")
                        .HasColumnType("integer");

                    b.Property<int>("Y")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EventTypeId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("BlueScore")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MvpId")
                        .HasColumnType("integer");

                    b.Property<int>("RedScore")
                        .HasColumnType("integer");

                    b.Property<Guid>("SeasonId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StateId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MvpId");

                    b.HasIndex("SeasonId");

                    b.HasIndex("StateId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.GamePlayer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Assists")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uuid");

                    b.Property<int>("Goals")
                        .HasColumnType("integer");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsCaptain")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Ping")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<int>("Team")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GamePlayers");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.NicknameChanges", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OldNickname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("NicknameChanges");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("NotificationsId")
                        .HasColumnType("uuid");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("NotificationsId");

                    b.HasIndex("RoleId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.PlayerNotificationSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("GameEnded")
                        .HasColumnType("integer");

                    b.Property<int>("GameStarted")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LogsCount")
                        .HasColumnType("integer");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PlayerNotificationSettings");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayChat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("Packet")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("ReplayDataId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ReplayDataId");

                    b.ToTable("ReplayChats");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Max")
                        .HasColumnType("bigint");

                    b.Property<long>("Min")
                        .HasColumnType("bigint");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("ReplayData");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayFragment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Index")
                        .HasColumnType("integer");

                    b.Property<long>("Max")
                        .HasColumnType("bigint");

                    b.Property<long>("Min")
                        .HasColumnType("bigint");

                    b.Property<Guid>("ReplayDataId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ReplayDataId");

                    b.ToTable("ReplayFragments");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayGoal", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("GoalBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Packet")
                        .HasColumnType("bigint");

                    b.Property<int>("Period")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ReplayDataId")
                        .HasColumnType("uuid");

                    b.Property<int>("Time")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReplayDataId");

                    b.ToTable("ReplayGoals");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Reports", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("FromId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ToId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FromId");

                    b.HasIndex("ToId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Season", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Server", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("BlueScore")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("LoggedIn")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Period")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerCount")
                        .HasColumnType("integer");

                    b.Property<int>("RedScore")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<int>("TeamMax")
                        .HasColumnType("integer");

                    b.Property<int>("Time")
                        .HasColumnType("integer");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DiscordNotificationWebhook")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("NewPlayerApproveRequired")
                        .HasColumnType("boolean");

                    b.Property<int>("NextGameCheckGames")
                        .HasColumnType("integer");

                    b.Property<int>("NicknameChangeDaysLimit")
                        .HasColumnType("integer");

                    b.Property<string>("PushJson")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ReplayStoreDays")
                        .HasColumnType("integer");

                    b.Property<string>("Rules")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("S3Bucket")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("S3Domain")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("S3Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("S3User")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ShadowBanReportsCount")
                        .HasColumnType("integer");

                    b.Property<int>("StartingElo")
                        .HasColumnType("integer");

                    b.Property<int>("WebhookCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.States", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("States");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Avatar", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Bans", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "BannedPlayer")
                        .WithMany("Bans")
                        .HasForeignKey("BannedPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BannedPlayer");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Elo", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.Season", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Season");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.EventWinners", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Events", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Events", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.EventType", "EventType")
                        .WithMany()
                        .HasForeignKey("EventTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EventType");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Game", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "Mvp")
                        .WithMany()
                        .HasForeignKey("MvpId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.Season", "Season")
                        .WithMany("Games")
                        .HasForeignKey("SeasonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.States", "State")
                        .WithMany()
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mvp");

                    b.Navigation("Season");

                    b.Navigation("State");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.GamePlayer", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Game", "Game")
                        .WithMany("GamePlayers")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "Player")
                        .WithMany("GamePlayers")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.NicknameChanges", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "Player")
                        .WithMany("NicknameChanges")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Player", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.PlayerNotificationSetting", "Notifications")
                        .WithMany()
                        .HasForeignKey("NotificationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Notifications");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayChat", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.ReplayData", null)
                        .WithMany("ReplayChats")
                        .HasForeignKey("ReplayDataId");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayData", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Game", "Game")
                        .WithMany("ReplayDatas")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayFragment", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.ReplayData", "ReplayData")
                        .WithMany("ReplayFragments")
                        .HasForeignKey("ReplayDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReplayData");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayGoal", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.ReplayData", null)
                        .WithMany("ReplayGoals")
                        .HasForeignKey("ReplayDataId");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Reports", b =>
                {
                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "From")
                        .WithMany()
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hqm_ranked_backend.Models.DbModels.Player", "To")
                        .WithMany()
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("From");

                    b.Navigation("To");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Game", b =>
                {
                    b.Navigation("GamePlayers");

                    b.Navigation("ReplayDatas");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Player", b =>
                {
                    b.Navigation("Bans");

                    b.Navigation("GamePlayers");

                    b.Navigation("NicknameChanges");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.ReplayData", b =>
                {
                    b.Navigation("ReplayChats");

                    b.Navigation("ReplayFragments");

                    b.Navigation("ReplayGoals");
                });

            modelBuilder.Entity("hqm_ranked_backend.Models.DbModels.Season", b =>
                {
                    b.Navigation("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
