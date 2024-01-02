namespace AddMemberSystem.Models
{
    public class TB_UserType
    {
        [Key]
        public int UserTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserType { get; set; }
    }
}
