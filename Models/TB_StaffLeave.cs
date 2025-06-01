using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_StaffLeave
    {
        [Key]
        public int StaffLeavePkid { get; set; }

        [MaxLength(50)]
        public string StaffID { get; set; }

        [MaxLength(50)]
        public string StaffLeaveName { get; set; }
 
        public DateTime? LeaveDateFrom { get; set; }

        public DateTime? LeaveDateTo { get; set; }

        [Required(ErrorMessage = "ခွင့်ရက်ရွေးဖို့ လိုအပ်ပါသည်")]
        public int LeaveDays { get; set; }

        public string LeaveAddress { get; set; }

        public string DutyAssignedTo { get; set; }

        public string DutyAssignPosition { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        [Required(ErrorMessage = "ခွင့်အမျိုးအစားရွေးဖို့ လိုအပ်ပါသည်")]
        [ForeignKey("LeaveTypePkid")]
        public int LeaveTypeId { get; set; }

        [NotMapped]
        public string CurrentDepartment { get; set; }

        [NotMapped]
        public string CurrentPosition { get; set; }

        public virtual TB_LeaveType LeaveType { get; set; }
    }
}
