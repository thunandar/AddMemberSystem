using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_Position
    {
        [Key]
        public int PositionPkid { get; set; }

        [MaxLength(50)]
        public string Position { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }



    }
}
