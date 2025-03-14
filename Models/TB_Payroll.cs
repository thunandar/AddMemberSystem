using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_Payroll
    {
        [Key]
        public int PayrollPkid { get; set; }

        [MaxLength(50)]
        public string StaffID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalSalary { get; set; }

        public DateTime? PaymentDate { get; set; }

        public bool IsDeleted { get; set; }

   
    }
}
