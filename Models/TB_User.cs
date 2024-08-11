using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_User
    {
        [Key]
        public int UserPkid { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        [Required]
        public string Password { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }


    }
}
