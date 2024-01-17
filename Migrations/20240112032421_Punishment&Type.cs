using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddMemberSystem.Migrations
{
    public partial class PunishmentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_PunishmentTypes",
                columns: table => new
                {
                    PunishmentTypePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    punishmentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PunishmentTypes", x => x.PunishmentTypePkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_StaffPunishments",
                columns: table => new
                {
                    StaffPunishmentPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    PunishmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Punishment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PunishmentTypeId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_StaffPunishments", x => x.StaffPunishmentPkid);
                    table.ForeignKey(
                        name: "FK_TB_StaffPunishments_TB_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TB_Departments",
                        principalColumn: "DepartmentPkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_StaffPunishments_TB_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "TB_Positions",
                        principalColumn: "PositionPkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_StaffPunishments_TB_PunishmentTypes_PunishmentTypeId",
                        column: x => x.PunishmentTypeId,
                        principalTable: "TB_PunishmentTypes",
                        principalColumn: "PunishmentTypePkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_StaffPunishments_TB_Staffs_StaffId",
                        column: x => x.StaffId,
                        principalTable: "TB_Staffs",
                        principalColumn: "StaffPkid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffPunishments_DepartmentId",
                table: "TB_StaffPunishments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffPunishments_PositionId",
                table: "TB_StaffPunishments",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffPunishments_PunishmentTypeId",
                table: "TB_StaffPunishments",
                column: "PunishmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffPunishments_StaffId",
                table: "TB_StaffPunishments",
                column: "StaffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_StaffPunishments");

            migrationBuilder.DropTable(
                name: "TB_PunishmentTypes");
        }
    }
}
