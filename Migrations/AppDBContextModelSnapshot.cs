﻿// <auto-generated />
using System;
using AddMemberSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AddMemberSystem.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AddMemberSystem.Models.TB_Department", b =>
                {
                    b.Property<int>("DepartmentPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentPkid"), 1L, 1);

                    b.Property<string>("Department")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("DepartmentPkid");

                    b.ToTable("TB_Departments");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_FuelType", b =>
                {
                    b.Property<int>("FuelTypePkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FuelTypePkid"), 1L, 1);

                    b.Property<string>("FuelType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("FuelTypePkid");

                    b.ToTable("TB_FuelTypes");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_LeaveType", b =>
                {
                    b.Property<int>("LeaveTypePkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeaveTypePkid"), 1L, 1);

                    b.Property<string>("LeaveTypeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LeaveTypePkid");

                    b.ToTable("TB_LeaveTypes");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_Manufacturer", b =>
                {
                    b.Property<int>("ManufacturerPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ManufacturerPkid"), 1L, 1);

                    b.Property<string>("Manufacturer")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ManufacturerPkid");

                    b.ToTable("TB_Manufacturers");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_Position", b =>
                {
                    b.Property<int>("PositionPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PositionPkid"), 1L, 1);

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("PositionPkid");

                    b.HasIndex("DepartmentId");

                    b.ToTable("TB_Positions");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_Staff", b =>
                {
                    b.Property<int>("StaffPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StaffPkid"), 1L, 1);

                    b.Property<string>("Address")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Age")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("FatherName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NRC")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PositionId")
                        .HasColumnType("int");

                    b.Property<string>("Religion")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Remarks")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Responsibility")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SerialNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StaffID")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("StaffPhoto")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("StartedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("VisibleMark")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("isDeleted")
                        .HasColumnType("bit");

                    b.HasKey("StaffPkid");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("PositionId");

                    b.ToTable("TB_Staffs");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_StaffLeave", b =>
                {
                    b.Property<int>("StaffLeavePkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StaffLeavePkid"), 1L, 1);

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("DutyAssignPosition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DutyAssignedTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LeaveAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LeaveDateFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LeaveDateTo")
                        .HasColumnType("datetime2");

                    b.Property<int>("LeaveDays")
                        .HasColumnType("int");

                    b.Property<int>("LeaveTypeId")
                        .HasColumnType("int");

                    b.Property<int>("PositionId")
                        .HasColumnType("int");

                    b.Property<string>("SerialNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StaffLeaveName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("StaffLeavePkid");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("LeaveTypeId");

                    b.HasIndex("PositionId");

                    b.ToTable("TB_StaffLeaves");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_User", b =>
                {
                    b.Property<int>("UserPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserPkid"), 1L, 1);

                    b.Property<string>("DisplayName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserID")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserPkid");

                    b.ToTable("TB_Users");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_UserType", b =>
                {
                    b.Property<int>("UserTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserTypeID"), 1L, 1);

                    b.Property<string>("UserType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserTypeID");

                    b.ToTable("TB_UserTypes");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_VehicleData", b =>
                {
                    b.Property<int>("VehicleDataPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VehicleDataPkid"), 1L, 1);

                    b.Property<string>("AssignedRoute")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("CctvInstalled")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CngQty")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FuelTypePkid")
                        .HasColumnType("int");

                    b.Property<string>("ManufacturedYear")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("POSInstalled")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Remarks")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("SerialNo")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("TelematicDeviceInstalled")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("TotalBusStop")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("VehicleManufacturer")
                        .HasColumnType("int");

                    b.Property<string>("VehicleNumber")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("VehicleTypePkid")
                        .HasColumnType("int");

                    b.Property<int>("YBSCompanyPkid")
                        .HasColumnType("int");

                    b.Property<string>("YBSName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("VehicleDataPkid");

                    b.ToTable("TB_VehicleDatas");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_YBSCompany", b =>
                {
                    b.Property<int>("YBSCompanyPkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("YBSCompanyPkid"), 1L, 1);

                    b.Property<string>("YBSCompany")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("YBSCompanyPkid");

                    b.ToTable("TB_YBSCompanies");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_YBSType", b =>
                {
                    b.Property<int>("YBSTypePkid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("YBSTypePkid"), 1L, 1);

                    b.Property<string>("YBSType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("YBSTypePkid");

                    b.ToTable("TB_YBSTypes");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_Position", b =>
                {
                    b.HasOne("AddMemberSystem.Models.TB_Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_Staff", b =>
                {
                    b.HasOne("AddMemberSystem.Models.TB_Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AddMemberSystem.Models.TB_Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("AddMemberSystem.Models.TB_StaffLeave", b =>
                {
                    b.HasOne("AddMemberSystem.Models.TB_Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AddMemberSystem.Models.TB_LeaveType", "LeaveType")
                        .WithMany()
                        .HasForeignKey("LeaveTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AddMemberSystem.Models.TB_Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("LeaveType");

                    b.Navigation("Position");
                });
#pragma warning restore 612, 618
        }
    }
}
