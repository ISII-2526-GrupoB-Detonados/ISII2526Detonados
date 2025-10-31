using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AppForSEII2526.API.DTOs.Purchase_DTO
{
    public class Purchase_ForCreate_DTO
    {
        public Purchase_ForCreate_DTO(string customerUserName, string customerNameSurname, string deliveryAddress,
                                    PaymentMethod paymentMethod, IList<Purchase_Item_DTO> purchaseItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(deliveryAddress));
            PaymentMethod = paymentMethod;
            PurchaseItems = purchaseItems ?? throw new ArgumentNullException(nameof(purchaseItems));
        }

        public Purchase_ForCreate_DTO()
        {
            PurchaseItems = new List<Purchase_Item_DTO>();
        }

        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        [Display(Name = "Delivery Address")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Delivery address must have at least 10 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your address for delivery")]
        public string DeliveryAddress { get; set; }

        [EmailAddress]
        [Required]
        public string CustomerUserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your Name and Surname")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Name and Surname must have at least 10 characters")]
        public string CustomerNameSurname { get; set; }

        public IList<Purchase_Item_DTO> PurchaseItems { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Total Price")]
        [JsonPropertyName("TotalPrice")]
        public double TotalPrice
        {
            get
            {
                return PurchaseItems.Sum(pi => pi.Price * pi.Quantity);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Purchase_ForCreate_DTO dTO &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   CustomerUserName == dTO.CustomerUserName &&
                   CustomerNameSurname == dTO.CustomerNameSurname &&
                   PaymentMethod == dTO.PaymentMethod &&
                   PurchaseItems.SequenceEqual(dTO.PurchaseItems) &&
                   TotalPrice == dTO.TotalPrice;
        }
    }
}