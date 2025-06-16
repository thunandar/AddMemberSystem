using System.ComponentModel.DataAnnotations.Schema;
using FoolProof;
using FoolProof.Core;

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

        [MaxLength(50)]
        public string MotherName { get; set; }

        [MaxLength(100)]
        public string LevelOfEducation { get; set; }

        public string SpouseAndChildrenNames { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(50)]
        public string NRC { get; set; }

        [MaxLength(10)]
        public string Age { get; set; }

        public string Gender { get; set; }


        [MaxLength(50)]
        public string Religion { get; set; }

        [MaxLength(50)]
        public string VisibleMark { get; set; }

        [MaxLength(300)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

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

        public bool SocialSecurity { get; set; } 

        public bool RiceOil { get; set; }

        [ForeignKey("StaffBenefitPkid")]
        [RequiredIfTrue("RiceOil", ErrorMessage = "အကျိုးခံစားခွင့် အမျိုးအစား ရွေးချယ်ရန် လိုအပ်ပါသည်")]
        public int? StaffBenefitId { get; set; }
        public virtual TB_StaffBenefit StaffBenefit { get; set; }

        public bool ChangeAmount { get; set; }

        [RequiredIfTrue("ChangeAmount", ErrorMessage = "ပမာဏ ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? CustomBenefitAmount { get; set; }


        [RequiredIfTrue("SocialSecurity", ErrorMessage = "ErSSN ထည့်ရန် လိုအပ်ပါသည်")]
        public string? ErSSN { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "EeSSN ထည့်ရန် လိုအပ်ပါသည်")]
        public string? EeSSN { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "Minc ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? Minc { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS1EeRate ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS1EeRate { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS1ErRate အမျိုးအစား ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS1ErRate { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS1EeConAmt ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS1EeConAmt { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS1ErConAmt ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS1ErConAmt { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS2EeRate ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS2EeRate { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS2ErRate ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS2ErRate { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS2EeConAmt ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS2EeConAmt { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "SS2ErConAmt ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? SS2ErConAmt { get; set; }

        [RequiredIfTrue("SocialSecurity", ErrorMessage = "TotalConAmt ထည့်ရန် လိုအပ်ပါသည်")]
        public decimal? TotalConAmt { get; set; }

        [NotMapped]
        public string CurrentDepartment { get; set; }

        [NotMapped]
        public string CurrentPosition { get; set; }

        public bool isDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }

    }
}
