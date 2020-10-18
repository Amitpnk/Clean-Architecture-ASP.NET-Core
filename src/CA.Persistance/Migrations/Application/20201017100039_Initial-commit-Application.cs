using Microsoft.EntityFrameworkCore.Migrations;
using System;

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
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CardId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SerialNumber = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Chapter = table.Column<int>(maxLength: 2, nullable: false),
                    Verse = table.Column<int>(maxLength: 2, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Synonmys = table.Column<string>(nullable: true),
                    Meaning = table.Column<string>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "Id", "Chapter", "Description", "GroupId", "Meaning", "Synonmys", "Verse" },
                values: new object[,]
                {
                    { new Guid("32fcf429-6982-49ba-ad30-ae4818d90c96"), 1, "dhrtarastra uvaca dharma-ksetre kuru-ksetre samaveta yuyutsavah    mamakah pandavas caiva    kim akurvata sanjaya", null, "Dhrtarastra said: O Sanjaya, after assembling in the place of pilgrimage at Kuruksetra, what did my sons and the sons of Pandu do, being desirous to fight?", "sanjayah--Sanjaya; uvaca--said; drstva--after seeing; tu--but; pandavaanikam--the soldiers of the Pandavas; vyudham--arranged in military phalanx; duryodhanah--King Duryodhana; tada--at that time; acaryam--the teacher; upasangamya--approaching nearby; raja--the king; vacanam--words; abravit--spoke.", 1 },
                    { new Guid("c983bb6a-8f1e-4b1b-a1b6-2751375f80ad"), 1, "sanjaya uvaca    drstva tu pandavanikam    vyudham duryodhanas tada    acaryam upasangamya    raja vacanam abravit", null, "Sanjaya said: O King, after looking over the army gathered by the sons of Pandu, King Duryodhana went to his teacher and began to speak the following words:", "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu; acarya--O teacher; mahatim--great; camum--military force; vyudham--arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--disciple; dhi-mata--very intelligent.", 2 },
                    { new Guid("96d152e7-402e-47c9-9fae-612c077acc25"), 1, "pasyaitam pandu-putranam     acarya mahatim camum    vyudham drupada-putrena    tava sisyena dhimata", null, "O my teacher, behold the great army of the sons of Pandu, so expertly    arranged by your intelligent disciple, the son of Drupada.", "pasya--behold; etam--this; pandu-putranam--of the sons of Pandu;   acarya--O teacher; mahatim--great; camum--military force; vyudham--    arranged; drupada-putrena--by the son of Drupada; tava--your; sisyena--    disciple; dhi-mata--very intelligent.", 3 }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CardId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("28da7440-0ee6-42ac-a683-31d170c9e693"), new Guid("00000000-0000-0000-0000-000000000000"), "Anger", true, "Anger" },
                    { new Guid("cfe4e5b7-e2c1-402f-ba67-a367e886139f"), new Guid("00000000-0000-0000-0000-000000000000"), "Confusion", true, "Confusion" },
                    { new Guid("de5f276c-bfc2-4724-b306-cc583614df62"), new Guid("00000000-0000-0000-0000-000000000000"), "Envy", true, "Envy" }
                });

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
