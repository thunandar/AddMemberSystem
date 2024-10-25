using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_Staff
    {
        [Key]
        public int StaffPkid { get; set; }

        public string SerialNo { get; set; }

        [MaxLength(50)]
        public string StaffID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string FatherName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "မှတ်ပုံတင်ထည့်ဖို့ လိုအပ်ပါသည်")]
        [MaxLength(50)]
        public string NRC { get; set; }

        [MaxLength(10)]
        public string Age { get; set; }

        [MaxLength(50)]
        public string Religion { get; set; }

        [MaxLength(50)]
        public string VisibleMark { get; set; }

        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [ForeignKey("InitialPositionPkid")]
        public int? InitialPositionId { get; set; }
        public virtual TB_InitialPosition InitialPosition { get; set; }
       
        [ForeignKey("PositionPkid")]
        public int? PositionId { get; set; }
        public virtual TB_Position Position { get; set; }
      
        [ForeignKey("DepartmentPkid")]
        public int DepartmentId{ get; set; }
        public virtual TB_Department Department { get; set; }

        [MaxLength(100)]
        public string Responsibility { get; set; }

        public DateTime? StartedDate { get; set; }

        [MaxLength(300)]
        public string Remarks { get; set; }


        [MaxLength(255, ErrorMessage = "Staff Image cannot exceed 255 characters.")]
        public string StaffPhoto { get; set; }

        public string Salary { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public bool isDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
