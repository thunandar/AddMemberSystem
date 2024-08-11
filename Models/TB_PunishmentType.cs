namespace AddMemberSystem.Models
{
    public class TB_PunishmentType
    {
        [Key]
        public int PunishmentTypePkid { get; set; }

        public string Punishment { get; set; }

        public bool IsDeleted { get; set; }
    }
}
