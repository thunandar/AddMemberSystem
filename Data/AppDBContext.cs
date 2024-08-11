using AddMemberSystem.Models;

namespace AddMemberSystem.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public DbSet<TB_Department> TB_Departments { get; set; }
        public DbSet<TB_FuelType> TB_FuelTypes { get; set; }
        public DbSet<TB_Manufacturer> TB_Manufacturers { get; set; }
        public DbSet<TB_Position> TB_Positions { get; set; }

        public DbSet<TB_InitialPosition> TB_InitialPositions { get; set; }

        public DbSet<TB_Staff> TB_Staffs { get; set; }
        public DbSet<TB_VehicleData> TB_VehicleDatas { get; set; }
        public DbSet<TB_YBSCompany> TB_YBSCompanies { get; set; }
        public DbSet<TB_YBSType> TB_YBSTypes { get; set; }
        public DbSet<TB_UserType> TB_UserTypes { get; set; }
        public DbSet<TB_User> TB_Users { get; set; }
        public DbSet<TB_LeaveType> TB_LeaveTypes { get; set; }
        public DbSet<TB_StaffLeave> TB_StaffLeaves { get; set; }
        public DbSet<TB_StaffPunishment> TB_StaffPunishments { get; set; }
        public DbSet<TB_PunishmentType> TB_PunishmentType { get; set; }

    }
}
