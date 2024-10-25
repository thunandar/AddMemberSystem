using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_StaffPunishment
    {
        [Key]
        public int StaffPunishmentPkid { get; set; }

        [Required(ErrorMessage = "၀န်ထမ်းအမှတ်ထည့်ဖို့ လိုအပ်ပါသည်")]
        [ForeignKey("StaffPkid")]
        public int StaffId { get; set; }
        public virtual TB_Staff Staff { get; set; }

        public DateTime? PunishmentDate { get; set; }

        public string Punishment { get; set; }

        [Required(ErrorMessage = "ပြစ်မှုအမျိုးအစားရွေးဖို့ လိုအပ်ပါသည်")]
        [ForeignKey("PunishmentTypePkid")]
        public int PunishmentTypeId { get; set; }
        public virtual TB_PunishmentType PunishmentType { get; set; }

        [ForeignKey("PositionId")]
        public int? PositionId { get; set; }
        public virtual TB_Position Position { get; set; }

        [Required(ErrorMessage = "ဌာနရွေးချယ်ဖို့ လိုအပ်ပါသည်")]
        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }
        public virtual TB_Department Department { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
