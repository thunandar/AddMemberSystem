using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_Position
    {
        [Key]
        public int PositionPkid { get; set; }

        [MaxLength(50)]
        public string Position { get; set; }

        public bool isDeleted { get; set; }

        [ForeignKey("DepartmentPkid")]
        public int DepartmentId { get; set; }

        public virtual TB_Department Department { get; set; }
    }
}
