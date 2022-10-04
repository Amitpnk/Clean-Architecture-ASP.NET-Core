using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CA.Persistance.Migrations.Application
{
    public partial class InitialcommitApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SerialNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chapter = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    Verse = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Synonmys = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "Id", "Chapter", "Description", "GroupId", "Meaning", "Synonmys", "Verse" },
                values: new object[,]
                {
                    { new Guid("2bf0bf73-aa20-4b68-9960-6ccb41b9585b"), 1, "dhrtarastra uvaca dharma-ksetre kuru-ksetre samaveta yuyutsavah    mamakah pandavas caiva    kim akurvata sanjaya", null, "Dhrtarastra said: O Sanjaya, after assembling in the place of pilgrimage at Kuruksetra, what did my sons and the sons of Pandu do, being desirous to fight?", "sanjayah--Sanjaya; uvaca--said; drstva--after seeing; tu--but; pandavaanikam--the soldiers of the Pandavas; vyudham--arranged in military phalanx; duryodhanah--King Duryodhana; tada--at that time; acaryam--the teacher; upasangamya--approaching nearby; raja--the king; vacanam--words; abravit--spoke.", 1 },
                    { new Guid("8afdbdfc-264d-4954-a7a4-93e5bc392839"), 1, "pasyaitam pandu-putranam     acarya mahatim camum    vyudham drupada-putrena    tava sisyena dhimata", null, "O my teacher, behold the great army of the sons of Pandu, so expertly    arranged by your intelligent disciple, the son of Drupada.", "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu;   acarya--O teacher; mahatim--great; camum--military force; vyudham--    arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--    disciple; dhi-mata--very intelligent.", 3 },
                    { new Guid("8ddc5538-e1e1-47ac-9205-1afc61eb74cf"), 1, "sanjaya uvaca    drstva tu pandavanikam    vyudham duryodhanas tada    acaryam upasangamya    raja vacanam abravit", null, "Sanjaya said: O King, after looking over the army gathered by the sons of Pandu, King Duryodhana went to his teacher and began to speak the following words:", "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu; acarya--O teacher; mahatim--great; camum--military force; vyudham--arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--disciple; dhi-mata--very intelligent.", 2 }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CardId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("ad46ae27-2a3e-4d9e-93d9-2046868a2862"), new Guid("00000000-0000-0000-0000-000000000000"), "Confusion", true, "Confusion" },
                    { new Guid("b0b643b3-47d9-4b11-8a0a-e64d6d5a959f"), new Guid("00000000-0000-0000-0000-000000000000"), "Anger", true, "Anger" },
                    { new Guid("b185b264-05c7-4131-9712-142e4316dabe"), new Guid("00000000-0000-0000-0000-000000000000"), "Envy", true, "Envy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Chapter_Verse",
                table: "Cards",
                columns: new[] { "Chapter", "Verse" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_GroupId",
                table: "Cards",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
