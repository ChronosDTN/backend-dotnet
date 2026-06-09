using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChronosDotnetApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_CDTN_NODE_REGISTRY",
                columns: table => new
                {
                    id_node = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    location = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    network_address = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CDTN_NODE_REGISTRY", x => x.id_node);
                });

            migrationBuilder.CreateTable(
                name: "T_CDTN_ASSET_BALANCES",
                columns: table => new
                {
                    id_asset = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    id_node = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    symbol = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    balance = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    last_update = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CDTN_ASSET_BALANCES", x => x.id_asset);
                    table.ForeignKey(
                        name: "FK_T_CDTN_ASSET_BALANCES_T_CDTN_NODE_REGISTRY_id_node",
                        column: x => x.id_node,
                        principalTable: "T_CDTN_NODE_REGISTRY",
                        principalColumn: "id_node",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_CDTN_DYNAMIC_ROUTES",
                columns: table => new
                {
                    id_route = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    source_node = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    target_node = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    bandwidth_kbps = table.Column<decimal>(type: "DECIMAL(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_CDTN_DYNAMIC_ROUTES", x => x.id_route);
                    table.ForeignKey(
                        name: "FK_T_CDTN_DYNAMIC_ROUTES_T_CDTN_NODE_REGISTRY_source_node",
                        column: x => x.source_node,
                        principalTable: "T_CDTN_NODE_REGISTRY",
                        principalColumn: "id_node",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_T_CDTN_DYNAMIC_ROUTES_T_CDTN_NODE_REGISTRY_target_node",
                        column: x => x.target_node,
                        principalTable: "T_CDTN_NODE_REGISTRY",
                        principalColumn: "id_node",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_CDTN_ASSET_BALANCES_id_node",
                table: "T_CDTN_ASSET_BALANCES",
                column: "id_node");

            migrationBuilder.CreateIndex(
                name: "IX_T_CDTN_DYNAMIC_ROUTES_source_node",
                table: "T_CDTN_DYNAMIC_ROUTES",
                column: "source_node");

            migrationBuilder.CreateIndex(
                name: "IX_T_CDTN_DYNAMIC_ROUTES_target_node",
                table: "T_CDTN_DYNAMIC_ROUTES",
                column: "target_node");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_CDTN_ASSET_BALANCES");

            migrationBuilder.DropTable(
                name: "T_CDTN_DYNAMIC_ROUTES");

            migrationBuilder.DropTable(
                name: "T_CDTN_NODE_REGISTRY");
        }
    }
}
