using System.ComponentModel.DataAnnotations.Schema;

namespace AddMemberSystem.Models
{
    public class TB_VehicleData
    {
        [Key]
        public int VehicleDataPkid { get; set; }

        [MaxLength(10)]
        public string SerialNo { get; set; }

        public int YBSCompanyPkid { get; set; }

        [MaxLength(50)]
        public string YBSName { get; set; }

        public int? VehicleManufacturer { get; set; }

        [MaxLength(50)]
        public string VehicleNumber { get; set; }

        [MaxLength(50)]
        public string ManufacturedYear { get; set; }

        public int? VehicleTypePkid { get; set; }

        public int? FuelTypePkid { get; set; }

        [MaxLength(50)]
        public string CngQty { get; set; }

        [MaxLength(50)]
        public string CctvInstalled { get; set; }

        [MaxLength(100)]
        public string AssignedRoute { get; set; }

        [MaxLength(50)]
        public string TelematicDeviceInstalled { get; set; }

        [MaxLength(50)]
        public string TotalBusStop { get; set; }

        [MaxLength(50)]
        public string POSInstalled { get; set; }

        public DateTime? RegistrationDate { get; set; }

        [MaxLength(300)]
        public string Remarks { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }
    }
}
