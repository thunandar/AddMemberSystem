namespace AddMemberSystem.Models
{
    public class TB_StaffBenefit
    {
        [Key]
        public int StaffBenefitPkid { get; set; }

        [MaxLength(50)]
        public string BenefitName { get; set; } 

        public string Amount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
