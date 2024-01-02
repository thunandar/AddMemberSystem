namespace AddMemberSystem.Models
{
    public class TB_YBSType
    {
        [Key]
        public int YBSTypePkid { get; set; }

        [MaxLength(50)]
        public string YBSType { get; set; }
    }
}
