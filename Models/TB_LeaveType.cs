namespace AddMemberSystem.Models
{
    public class TB_LeaveType
    {
        [Key]
        public int LeaveTypePkid { get; set; }

        public string LeaveTypeName { get; set; }

        public int LeaveDays { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
