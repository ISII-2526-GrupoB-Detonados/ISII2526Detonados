namespace AppForSEII2526.API.DTOs.Rentals_DTO
{
    public class Rental_Item_DTO
    {
        //------------------ Constructors ------------------
        public Rental_Item_DTO()
        {

        }

        public Rental_Item_DTO(int deviceQuantity, string deviceModel, int deviceId, string deviceName, double priceForRenting)
        {
            DeviceQuantity = deviceQuantity;
            DeviceModel = deviceModel;
            DeviceId = deviceId;
            DeviceName = deviceName;
            PriceForRenting = priceForRenting;
        }

        //------------------- Device Info ------------------
        // LA RESPONSABILIDAD DE LA CAPA INCLUYE LA DECLARACION DE LAS VALIDACIONES DE LOS ATRIBUTOS
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Device Quantity")]
        public int DeviceQuantity { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Device Model")]
        public string DeviceModel { get; set; }

        [Required]
        [Display(Name = "Device ID")]
        public int DeviceId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        [Display(Name = "Price for Renting")]
        public double PriceForRenting { get; set; }

        //------------------ Overload Equals y GetHashCode ------------------
        public override bool Equals(object? obj)
        {
            return obj is Rental_Item_DTO dTO &&
                   DeviceQuantity == dTO.DeviceQuantity &&
                   DeviceModel == dTO.DeviceModel &&
                   DeviceId == dTO.DeviceId &&
                   DeviceName == dTO.DeviceName &&
                   PriceForRenting == dTO.PriceForRenting;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceQuantity, DeviceModel, DeviceId, DeviceName, PriceForRenting);
        }
    }
}
