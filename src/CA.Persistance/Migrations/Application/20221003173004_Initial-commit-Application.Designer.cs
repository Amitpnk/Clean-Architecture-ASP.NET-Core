﻿// <auto-generated />
using System;
using CA.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CA.Persistance.Migrations.Application
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20221003173004_Initial-commit-Application")]
    partial class InitialcommitApplication
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CA.Domain.Entities.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Chapter")
                        .HasMaxLength(2)
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Meaning")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SerialNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SerialNumber"), 1L, 1);

                    b.Property<string>("Synonmys")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Verse")
                        .HasMaxLength(2)
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("Chapter", "Verse")
                        .IsUnique();

                    b.ToTable("Cards");

                    b.HasData(
                        new
                        {
                            Id = new Guid("2bf0bf73-aa20-4b68-9960-6ccb41b9585b"),
                            Chapter = 1,
                            Description = "dhrtarastra uvaca dharma-ksetre kuru-ksetre samaveta yuyutsavah    mamakah pandavas caiva    kim akurvata sanjaya",
                            Meaning = "Dhrtarastra said: O Sanjaya, after assembling in the place of pilgrimage at Kuruksetra, what did my sons and the sons of Pandu do, being desirous to fight?",
                            SerialNumber = 0,
                            Synonmys = "sanjayah--Sanjaya; uvaca--said; drstva--after seeing; tu--but; pandavaanikam--the soldiers of the Pandavas; vyudham--arranged in military phalanx; duryodhanah--King Duryodhana; tada--at that time; acaryam--the teacher; upasangamya--approaching nearby; raja--the king; vacanam--words; abravit--spoke.",
                            Verse = 1
                        },
                        new
                        {
                            Id = new Guid("8ddc5538-e1e1-47ac-9205-1afc61eb74cf"),
                            Chapter = 1,
                            Description = "sanjaya uvaca    drstva tu pandavanikam    vyudham duryodhanas tada    acaryam upasangamya    raja vacanam abravit",
                            Meaning = "Sanjaya said: O King, after looking over the army gathered by the sons of Pandu, King Duryodhana went to his teacher and began to speak the following words:",
                            SerialNumber = 0,
                            Synonmys = "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu; acarya--O teacher; mahatim--great; camum--military force; vyudham--arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--disciple; dhi-mata--very intelligent.",
                            Verse = 2
                        },
                        new
                        {
                            Id = new Guid("8afdbdfc-264d-4954-a7a4-93e5bc392839"),
                            Chapter = 1,
                            Description = "pasyaitam pandu-putranam     acarya mahatim camum    vyudham drupada-putrena    tava sisyena dhimata",
                            Meaning = "O my teacher, behold the great army of the sons of Pandu, so expertly    arranged by your intelligent disciple, the son of Drupada.",
                            SerialNumber = 0,
                            Synonmys = "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu;   acarya--O teacher; mahatim--great; camum--military force; vyudham--    arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--    disciple; dhi-mata--very intelligent.",
                            Verse = 3
                        });
                });

            modelBuilder.Entity("CA.Domain.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CardId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Groups");

                    b.HasData(
                        new
                        {
                            Id = new Guid("b0b643b3-47d9-4b11-8a0a-e64d6d5a959f"),
                            CardId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Description = "Anger",
                            IsActive = true,
                            Name = "Anger"
                        },
                        new
                        {
                            Id = new Guid("ad46ae27-2a3e-4d9e-93d9-2046868a2862"),
                            CardId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Description = "Confusion",
                            IsActive = true,
                            Name = "Confusion"
                        },
                        new
                        {
                            Id = new Guid("b185b264-05c7-4131-9712-142e4316dabe"),
                            CardId = new Guid("00000000-0000-0000-0000-000000000000"),
                            Description = "Envy",
                            IsActive = true,
                            Name = "Envy"
                        });
                });

            modelBuilder.Entity("CA.Domain.Entities.Card", b =>
                {
                    b.HasOne("CA.Domain.Entities.Group", "Group")
                        .WithMany("Cards")
                        .HasForeignKey("GroupId");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("CA.Domain.Entities.Group", b =>
                {
                    b.Navigation("Cards");
                });
#pragma warning restore 612, 618
        }
    }
}
