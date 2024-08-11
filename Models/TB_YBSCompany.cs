namespace AddMemberSystem.Models
{
    public class TB_YBSCompany
    {
        [Key]
        public int YBSCompanyPkid { get; set; }

        [MaxLength(100)]
        public string YBSCompany { get; set; }
    }
}
