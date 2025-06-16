using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddMemberSystem.Migrations
{
    public partial class createDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Departments",
                columns: table => new
                {
                    DepartmentPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SerialNo = table.Column<int>(type: "int", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Departments", x => x.DepartmentPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_FuelTypes",
                columns: table => new
                {
                    FuelTypePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_FuelTypes", x => x.FuelTypePkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_LeaveTypes",
                columns: table => new
                {
                    LeaveTypePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeaveDays = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_LeaveTypes", x => x.LeaveTypePkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_Manufacturers",
                columns: table => new
                {
                    ManufacturerPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Manufacturers", x => x.ManufacturerPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_Payrolls",
                columns: table => new
                {
                    PayrollPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SocialSecurityDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RiceOilDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeaveDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Deductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthOfSalary = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YearOfSalary = table.Column<int>(type: "int", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Payrolls", x => x.PayrollPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_Positions",
                columns: table => new
                {
                    PositionPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNo = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Positions", x => x.PositionPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_PunishmentType",
                columns: table => new
                {
                    PunishmentTypePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Punishment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PunishmentType", x => x.PunishmentTypePkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_Salaries",
                columns: table => new
                {
                    SalaryPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SocialSecurityDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RiceOilDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeaveDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Deductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MonthOfSalary = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YearOfSalary = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Salaries", x => x.SalaryPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_StaffBenefit",
                columns: table => new
                {
                    StaffBenefitPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BenefitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_StaffBenefit", x => x.StaffBenefitPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_Users",
                columns: table => new
                {
                    UserPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Users", x => x.UserPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_UserTypes",
                columns: table => new
                {
                    UserTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_UserTypes", x => x.UserTypeID);
                });

            migrationBuilder.CreateTable(
                name: "TB_VehicleDatas",
                columns: table => new
                {
                    VehicleDataPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    YBSCompanyPkid = table.Column<int>(type: "int", nullable: false),
                    YBSName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleManufacturer = table.Column<int>(type: "int", nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ManufacturedYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VehicleTypePkid = table.Column<int>(type: "int", nullable: true),
                    FuelTypePkid = table.Column<int>(type: "int", nullable: true),
                    CngQty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CctvInstalled = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssignedRoute = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelematicDeviceInstalled = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalBusStop = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    POSInstalled = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VehicleDatas", x => x.VehicleDataPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_YBSCompanies",
                columns: table => new
                {
                    YBSCompanyPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YBSCompany = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_YBSCompanies", x => x.YBSCompanyPkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_YBSTypes",
                columns: table => new
                {
                    YBSTypePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YBSType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_YBSTypes", x => x.YBSTypePkid);
                });

            migrationBuilder.CreateTable(
                name: "TB_StaffLeaves",
                columns: table => new
                {
                    StaffLeavePkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StaffLeaveName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LeaveDateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaveDateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaveDays = table.Column<int>(type: "int", nullable: false),
                    LeaveAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DutyAssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DutyAssignPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_StaffLeaves", x => x.StaffLeavePkid);
                    table.ForeignKey(
                        name: "FK_TB_StaffLeaves_TB_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "TB_LeaveTypes",
                        principalColumn: "LeaveTypePkid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_JobHistorys",
                columns: table => new
                {
                    JobHistoryPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JobYear = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    JobMonth = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    JobDay = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_JobHistorys", x => x.JobHistoryPkid);
                    table.ForeignKey(
                        name: "FK_TB_JobHistorys_TB_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "TB_Departments",
                        principalColumn: "DepartmentPkid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_JobHistorys_TB_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "TB_Positions",
                        principalColumn: "PositionPkid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_Staffs",
                columns: table => new
                {
                    StaffPkid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaffID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LevelOfEducation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpouseAndChildrenNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NRC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Age = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VisibleMark = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Responsibility = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    StaffPhoto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Salary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialSecurity = table.Column<bool>(type: "bit", nullable: false),
                    RiceOil = table.Column<bool>(type: "bit", nullable: false),
                    StaffBenefitId = table.Column<int>(type: "int", nullable: true),
                    ChangeAmount = table.Column<bool>(type: "bit", nullable: false),
                    CustomBenefitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ErSSN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EeSSN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Minc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS1EeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS1ErRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS1EeConAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS1ErConAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS2EeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS2ErRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS2EeConAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SS2ErConAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalConAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Staffs", x => x.StaffPkid);
                    table.ForeignKey(
                        name: "FK_TB_Staffs_TB_StaffBenefit_StaffBenefitId",
                        column: x => x.StaffBenefitId,
                        principalTable: "TB_StaffBenefit",
                        principalColumn: "StaffBenefitPkid");
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
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_StaffPunishments", x => x.StaffPunishmentPkid);
                    table.ForeignKey(
                        name: "FK_TB_StaffPunishments_TB_PunishmentType_PunishmentTypeId",
                        column: x => x.PunishmentTypeId,
                        principalTable: "TB_PunishmentType",
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
                name: "IX_TB_JobHistorys_DepartmentId",
                table: "TB_JobHistorys",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_JobHistorys_PositionId",
                table: "TB_JobHistorys",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffLeaves_LeaveTypeId",
                table: "TB_StaffLeaves",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffPunishments_PunishmentTypeId",
                table: "TB_StaffPunishments",
                column: "PunishmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_StaffPunishments_StaffId",
                table: "TB_StaffPunishments",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Staffs_StaffBenefitId",
                table: "TB_Staffs",
                column: "StaffBenefitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_FuelTypes");

            migrationBuilder.DropTable(
                name: "TB_JobHistorys");

            migrationBuilder.DropTable(
                name: "TB_Manufacturers");

            migrationBuilder.DropTable(
                name: "TB_Payrolls");

            migrationBuilder.DropTable(
                name: "TB_Salaries");

            migrationBuilder.DropTable(
                name: "TB_StaffLeaves");

            migrationBuilder.DropTable(
                name: "TB_StaffPunishments");

            migrationBuilder.DropTable(
                name: "TB_Users");

            migrationBuilder.DropTable(
                name: "TB_UserTypes");

            migrationBuilder.DropTable(
                name: "TB_VehicleDatas");

            migrationBuilder.DropTable(
                name: "TB_YBSCompanies");

            migrationBuilder.DropTable(
                name: "TB_YBSTypes");

            migrationBuilder.DropTable(
                name: "TB_Departments");

            migrationBuilder.DropTable(
                name: "TB_Positions");

            migrationBuilder.DropTable(
                name: "TB_LeaveTypes");

            migrationBuilder.DropTable(
                name: "TB_PunishmentType");

            migrationBuilder.DropTable(
                name: "TB_Staffs");

            migrationBuilder.DropTable(
                name: "TB_StaffBenefit");
        }
    }
}
