using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.Purchase_DTO
{
    public class Purchase_Detail_DTO : Purchase_ForCreate_DTO
    {
        public Purchase_Detail_DTO(int id, DateTime purchaseDate, string customerUserName, string customerNameSurname,
            string deliveryAddress, PaymentMethod paymentMethod, IList<Purchase_Item_DTO> purchaseItems)
            : base(customerUserName,
                   customerNameSurname,
                   deliveryAddress,
                   paymentMethod,
                   purchaseItems)
        {
            Id = id;
            PurchaseDate = purchaseDate;
        }

        public Purchase_Detail_DTO()
        {
        }

        public int Id { get; set; }

        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Total Quantity")]
        public int TotalQuantity
        {
            get
            {
                return PurchaseItems.Sum(pi => pi.Quantity);
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is Purchase_Detail_DTO dTO &&
                   base.Equals(obj) &&
                   TotalPrice == dTO.TotalPrice &&
                   Id == dTO.Id &&
                   CompareDate(PurchaseDate, dTO.PurchaseDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, PurchaseDate);
        }
    }
}
