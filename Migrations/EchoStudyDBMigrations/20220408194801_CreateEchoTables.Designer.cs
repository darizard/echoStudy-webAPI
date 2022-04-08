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
    [Migration("20220408194801_CreateEchoTables")]
    partial class CreateEchoTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.14")
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

                    b.Property<long>("DeckPosition")
                        .HasColumnType("bigint");

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

                    b.HasAlternateKey("DeckID", "DeckPosition");

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

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DeckID");

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
