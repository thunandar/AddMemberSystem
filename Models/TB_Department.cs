namespace AddMemberSystem.Models
{
    public class TB_Department
    {

        [Key]
        public int DepartmentPkid { get; set; }

        [MaxLength(200)]
        public string Department { get; set; }

        public bool isDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }

    }
}
