namespace AddMemberSystem.Models
{
    public class TB_FuelType
    {
        [Key]
        public int FuelTypePkid { get; set; }

        [MaxLength(50)]
        public string FuelType { get; set; }
    }
}
