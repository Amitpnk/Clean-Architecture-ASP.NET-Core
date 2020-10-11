using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CA.Persistance.Migrations.InitialSetup
{
    public partial class Initialsetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "1001", "Martin Fowler" },
                    { 2, "1002", "Uncle Bob" },
                    { 3, "1003", "Kent Beck" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Eric Evans" },
                    { 2, "Greg Young" },
                    { 3, "Udi Dahan" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Spaghetti", 5m },
                    { 2, "Lasagna", 10m },
                    { 3, "Ravioli", 15m }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CustomerId", "Date", "EmployeeId", "ProductId", "Quantity", "TotalPrice", "UnitPrice" },
                values: new object[] { 1, 1, new DateTime(2020, 9, 1, 0, 0, 0, 0, DateTimeKind.Local), 1, 1, 1, 5m, 5m });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CustomerId", "Date", "EmployeeId", "ProductId", "Quantity", "TotalPrice", "UnitPrice" },
                values: new object[] { 2, 2, new DateTime(2020, 9, 2, 0, 0, 0, 0, DateTimeKind.Local), 2, 2, 2, 20m, 10m });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CustomerId", "Date", "EmployeeId", "ProductId", "Quantity", "TotalPrice", "UnitPrice" },
                values: new object[] { 3, 3, new DateTime(2020, 9, 3, 0, 0, 0, 0, DateTimeKind.Local), 3, 3, 3, 45m, 15m });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_EmployeeId",
                table: "Sales",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ProductId",
                table: "Sales",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
