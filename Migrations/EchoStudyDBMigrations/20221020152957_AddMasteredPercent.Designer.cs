﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using echoStudy_webAPI.Models;

namespace echoStudy_webAPI.Migrations.EchoStudyDBMigrations
{
    [DbContext(typeof(EchoStudyDB))]
    [Migration("20221020152957_AddMasteredPercent")]
    partial class AddMasteredPercent
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DeckDeckCategory", b =>
                {
                    b.Property<int>("DeckCategoriesCategoryID")
                        .HasColumnType("int");

                    b.Property<int>("DecksDeckID")
                        .HasColumnType("int");

                    b.HasKey("DeckCategoriesCategoryID", "DecksDeckID");

                    b.HasIndex("DecksDeckID");

                    b.ToTable("DeckDeckCategory");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("echoStudy_webAPI.Areas.Identity.Data.EchoUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Card", b =>
                {
                    b.Property<int>("CardID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BackAudio")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<int>("BackLang")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("BackText")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTouched")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("DeckID")
                        .HasColumnType("int");

                    b.Property<string>("DeckPosition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FrontAudio")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<int>("FrontLang")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("FrontText")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<int?>("SessionID")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CardID");

                    b.HasIndex("DeckID");

                    b.HasIndex("SessionID");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Deck", b =>
                {
                    b.Property<int>("DeckID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Access")
                        .HasMaxLength(40)
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTouched")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("DefaultBackLang")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<int>("DefaultFrontLang")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<double?>("MasteredPercent")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("float");

                    b.Property<string>("OrigAuthorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("OrigDeckId")
                        .HasColumnType("int");

                    b.Property<double?>("StudyPercent")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DeckID");

                    b.HasIndex("OrigAuthorId");

                    b.HasIndex("OrigDeckId");

                    b.ToTable("Decks");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.DeckCategory", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTouched")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryID");

                    b.ToTable("DeckCategories");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.RefreshToken", b =>
                {
                    b.Property<string>("Token")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EchoUser")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("JwtId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Revoked")
                        .HasColumnType("bit");

                    b.Property<bool>("Used")
                        .HasColumnType("bit");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Token");

                    b.HasIndex("EchoUser");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Session", b =>
                {
                    b.Property<int>("SessionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateLastStudied")
                        .HasColumnType("datetime2");

                    b.Property<int>("DeckID")
                        .HasColumnType("int");

                    b.Property<string>("Device")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LearnReview")
                        .HasColumnType("int");

                    b.Property<int>("MaxCards")
                        .HasColumnType("int");

                    b.Property<int>("Platform")
                        .HasColumnType("int");

                    b.Property<int>("PlayOrder")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SessionID");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("DeckDeckCategory", b =>
                {
                    b.HasOne("echoStudy_webAPI.Models.DeckCategory", null)
                        .WithMany()
                        .HasForeignKey("DeckCategoriesCategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("echoStudy_webAPI.Models.Deck", null)
                        .WithMany()
                        .HasForeignKey("DecksDeckID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("echoStudy_webAPI.Areas.Identity.Data.EchoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("echoStudy_webAPI.Areas.Identity.Data.EchoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("echoStudy_webAPI.Areas.Identity.Data.EchoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("echoStudy_webAPI.Areas.Identity.Data.EchoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Card", b =>
                {
                    b.HasOne("echoStudy_webAPI.Models.Deck", "Deck")
                        .WithMany("Cards")
                        .HasForeignKey("DeckID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("echoStudy_webAPI.Models.Session", null)
                        .WithMany("CardsPlayed")
                        .HasForeignKey("SessionID");

                    b.Navigation("Deck");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Deck", b =>
                {
                    b.HasOne("echoStudy_webAPI.Areas.Identity.Data.EchoUser", "OrigAuthor")
                        .WithMany()
                        .HasForeignKey("OrigAuthorId");

                    b.HasOne("echoStudy_webAPI.Models.Deck", "OrigDeck")
                        .WithMany()
                        .HasForeignKey("OrigDeckId");

                    b.Navigation("OrigAuthor");

                    b.Navigation("OrigDeck");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.RefreshToken", b =>
                {
                    b.HasOne("echoStudy_webAPI.Areas.Identity.Data.EchoUser", "User")
                        .WithMany()
                        .HasForeignKey("EchoUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Deck", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("echoStudy_webAPI.Models.Session", b =>
                {
                    b.Navigation("CardsPlayed");
                });
#pragma warning restore 612, 618
        }
    }
}
