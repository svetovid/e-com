using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace btm.paas.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MethodActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MethodActions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Reference = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerReference = table.Column<string>(type: "TEXT", nullable: true),
                    MethodActionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicPaymentId = table.Column<string>(type: "TEXT", nullable: true),
                    ProviderAccountName = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Reference);
                    table.ForeignKey(
                        name: "FK_Payments_MethodActions_MethodActionId",
                        column: x => x.MethodActionId,
                        principalTable: "MethodActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentReference = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_Payments_PaymentReference",
                        column: x => x.PaymentReference,
                        principalTable: "Payments",
                        principalColumn: "Reference");
                });

            migrationBuilder.InsertData(
                table: "MethodActions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Deposit" },
                    { 2, "Withdrawal" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentReference",
                table: "PaymentHistories",
                column: "PaymentReference");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MethodActionId",
                table: "Payments",
                column: "MethodActionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "MethodActions");
        }
    }
}
