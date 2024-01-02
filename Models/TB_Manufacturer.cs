namespace AddMemberSystem.Models
{
    public class TB_Manufacturer
    {
        [Key]
        public int ManufacturerPkid { get; set; }

        [MaxLength(100)]
        public string Manufacturer { get; set; }
    }
}
