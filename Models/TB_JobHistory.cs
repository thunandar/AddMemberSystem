using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_JobHistory
    {
        [Key]
        public int JobHistoryPkid { get; set; }

        [Required]
        [MaxLength(50)]
        public string StaffID { get; set; }

        [ForeignKey("DepartmentPkid")]
        public int DepartmentId { get; set; }

        public virtual TB_Department Department { get; set; }

        [ForeignKey("PositionPkid")]
        public int PositionId { get; set; }

        public virtual TB_Position Position { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal? JobYear { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal? JobMonth { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal? JobDay { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Duration { get; set; }

        [MaxLength(500)]
        public string Remark { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
