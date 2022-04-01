using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace diplom2.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BufferVSM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinHold = table.Column<int>(type: "int", nullable: true),
                    Max = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: true),
                    ValueDefault = table.Column<int>(type: "int", nullable: true),
                    ReplenishmentSec = table.Column<int>(type: "int", nullable: true),
                    ReplenishmentCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BufferVSM", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EtapVSM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualTimeCircle = table.Column<float>(type: "real", nullable: true),
                    DefaultTimeCircle = table.Column<float>(type: "real", nullable: true),
                    ActualTimePreporation = table.Column<float>(type: "real", nullable: true),
                    DefaultTimePreporation = table.Column<float>(type: "real", nullable: true),
                    ActualAvailability = table.Column<float>(type: "real", nullable: true),
                    DefaultAvailability = table.Column<float>(type: "real", nullable: true),
                    Parallel = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapVSM", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderRole = table.Column<byte>(type: "tinyint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    TAdd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TStop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TActual = table.Column<float>(type: "real", nullable: true),
                    TPlan = table.Column<float>(type: "real", nullable: true),
                    TFuture = table.Column<float>(type: "real", nullable: true),
                    Simulation = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Patronymic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardVSM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionsId = table.Column<int>(type: "int", nullable: true),
                    EtapNumeric = table.Column<int>(type: "int", nullable: true),
                    EtapVSMId = table.Column<int>(type: "int", nullable: true),
                    BufferVSMId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardVSM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardVSM_BufferVSM_BufferVSMId",
                        column: x => x.BufferVSMId,
                        principalTable: "BufferVSM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardVSM_EtapVSM_EtapVSMId",
                        column: x => x.EtapVSMId,
                        principalTable: "EtapVSM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardVSM_Productions_ProductionsId",
                        column: x => x.ProductionsId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_production",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderRole = table.Column<byte>(type: "tinyint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Simulation = table.Column<bool>(type: "bit", nullable: true),
                    TAdd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TStop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TActual = table.Column<float>(type: "real", nullable: true),
                    TPlan = table.Column<float>(type: "real", nullable: true),
                    TFuture = table.Column<float>(type: "real", nullable: true),
                    ProductionsId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_production", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_production_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_production_Productions_ProductionsId",
                        column: x => x.ProductionsId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders_production_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderRole = table.Column<byte>(type: "tinyint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Part = table.Column<int>(type: "int", nullable: false),
                    TStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TStop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TDefault = table.Column<float>(type: "real", nullable: true),
                    TActual = table.Column<float>(type: "real", nullable: true),
                    TFuture = table.Column<float>(type: "real", nullable: true),
                    Simulation = table.Column<bool>(type: "bit", nullable: true),
                    Orders_productionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_production_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_production_items_Orders_production_Orders_productionId",
                        column: x => x.Orders_productionId,
                        principalTable: "Orders_production",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActualOrderCurrentSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderSectionState = table.Column<byte>(type: "tinyint", nullable: false),
                    ActualEtapVSMId = table.Column<int>(type: "int", nullable: true),
                    ActualEtapSectionsId = table.Column<int>(type: "int", nullable: true),
                    ActualBufferVSMId = table.Column<int>(type: "int", nullable: true),
                    Orders_production_itemsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualOrderCurrentSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActualOrderCurrentSection_Orders_production_items_Orders_production_itemsId",
                        column: x => x.Orders_production_itemsId,
                        principalTable: "Orders_production_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BufferVSMQueue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BufferRole = table.Column<byte>(type: "tinyint", nullable: false),
                    TAdd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TPop = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TWait = table.Column<float>(type: "real", nullable: true),
                    TFuture = table.Column<float>(type: "real", nullable: true),
                    Orders_production_itemsId = table.Column<int>(type: "int", nullable: true),
                    BufferVSMId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BufferVSMQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BufferVSMQueue_BufferVSM_BufferVSMId",
                        column: x => x.BufferVSMId,
                        principalTable: "BufferVSM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BufferVSMQueue_Orders_production_items_Orders_production_itemsId",
                        column: x => x.Orders_production_itemsId,
                        principalTable: "Orders_production_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EtapSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Work = table.Column<bool>(type: "bit", nullable: true),
                    TActual = table.Column<float>(type: "real", nullable: true),
                    TMax = table.Column<float>(type: "real", nullable: true),
                    TMin = table.Column<float>(type: "real", nullable: true),
                    EtapVSMId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Orders_production_itemsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtapSections_EtapVSM_EtapVSMId",
                        column: x => x.EtapVSMId,
                        principalTable: "EtapVSM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EtapSections_Orders_production_items_Orders_production_itemsId",
                        column: x => x.Orders_production_itemsId,
                        principalTable: "Orders_production_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EtapSections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArchiveSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveSectionRole = table.Column<byte>(type: "tinyint", nullable: false),
                    Time = table.Column<float>(type: "real", nullable: true),
                    EtapSectionsId = table.Column<int>(type: "int", nullable: true),
                    Orders_production_itemsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchiveSection_EtapSections_EtapSectionsId",
                        column: x => x.EtapSectionsId,
                        principalTable: "EtapSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArchiveSection_Orders_production_items_Orders_production_itemsId",
                        column: x => x.Orders_production_itemsId,
                        principalTable: "Orders_production_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActualOrderCurrentSection_Orders_production_itemsId",
                table: "ActualOrderCurrentSection",
                column: "Orders_production_itemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveSection_EtapSectionsId",
                table: "ArchiveSection",
                column: "EtapSectionsId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveSection_Orders_production_itemsId",
                table: "ArchiveSection",
                column: "Orders_production_itemsId");

            migrationBuilder.CreateIndex(
                name: "IX_BufferVSMQueue_BufferVSMId",
                table: "BufferVSMQueue",
                column: "BufferVSMId");

            migrationBuilder.CreateIndex(
                name: "IX_BufferVSMQueue_Orders_production_itemsId",
                table: "BufferVSMQueue",
                column: "Orders_production_itemsId");

            migrationBuilder.CreateIndex(
                name: "IX_CardVSM_BufferVSMId",
                table: "CardVSM",
                column: "BufferVSMId");

            migrationBuilder.CreateIndex(
                name: "IX_CardVSM_EtapVSMId",
                table: "CardVSM",
                column: "EtapVSMId");

            migrationBuilder.CreateIndex(
                name: "IX_CardVSM_ProductionsId",
                table: "CardVSM",
                column: "ProductionsId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapSections_EtapVSMId",
                table: "EtapSections",
                column: "EtapVSMId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapSections_Orders_production_itemsId",
                table: "EtapSections",
                column: "Orders_production_itemsId");

            migrationBuilder.CreateIndex(
                name: "IX_EtapSections_UserId",
                table: "EtapSections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_production_OrderId",
                table: "Orders_production",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_production_ProductionsId",
                table: "Orders_production",
                column: "ProductionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_production_items_Orders_productionId",
                table: "Orders_production_items",
                column: "Orders_productionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActualOrderCurrentSection");

            migrationBuilder.DropTable(
                name: "ArchiveSection");

            migrationBuilder.DropTable(
                name: "BufferVSMQueue");

            migrationBuilder.DropTable(
                name: "CardVSM");

            migrationBuilder.DropTable(
                name: "EtapSections");

            migrationBuilder.DropTable(
                name: "BufferVSM");

            migrationBuilder.DropTable(
                name: "EtapVSM");

            migrationBuilder.DropTable(
                name: "Orders_production_items");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Orders_production");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Productions");
        }
    }
}
