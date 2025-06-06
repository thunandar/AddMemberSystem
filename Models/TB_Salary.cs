using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_Salary
    {
        [Key]
        public int SalaryPkid { get; set; }

        [MaxLength(50)]
        public string StaffID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BaseSalary { get; set; }

        public decimal? SocialSecurityDeduction { get; set; }
        public decimal? RiceOilDeduction { get; set; }
        public decimal? LeaveDeduction { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal? Deductions { get; set; } //total deductions

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetSalary { get; set; }

        [MaxLength(50)]
        public string MonthOfSalary { get; set; }

        public int? YearOfSalary { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
