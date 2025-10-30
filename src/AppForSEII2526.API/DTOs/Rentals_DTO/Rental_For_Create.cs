namespace AppForSEII2526.API.DTOs.Rentals_DTO
{

    using DataType = System.ComponentModel.DataAnnotations.DataType;// Avoid ambiguity with other DataType definitions

    public class Rental_For_Create
    {
        public Rental_For_Create(string customerUserName, string customerNameSurname, string deliveryAddress, PaymentMethod paymentMethod, DateTime rentalDateFrom, DateTime rentalDateTo, IList<Rental_Item_DTO> rentalItems)
        {
            CustomerUserName = customerUserName ?? throw new ArgumentNullException(nameof(customerUserName));
            CustomerNameSurname = customerNameSurname ?? throw new ArgumentNullException(nameof(customerNameSurname));
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(deliveryAddress));
            PaymentMethod = paymentMethod;
            RentalDateFrom = rentalDateFrom;
            RentalDateTo = rentalDateTo;
            RentalItems = rentalItems ?? throw new ArgumentNullException(nameof(rentalItems));
        }

        public Rental_For_Create()
        {
            RentalItems = new List<Rental_Item_DTO>();
        }
        [Display(Name = "Fecha de alquiler")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RentalDateFrom { get; set; }
        [DataType(DataType.Date), Display(Name = "Fecha fin alquiler")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RentalDateTo { get; set; }


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

        public IList<Rental_Item_DTO> RentalItems { get; set; } //Ojo. cambio de clases aunque asi queda mas claro una vez se ve 
        [Required]
        public PaymentMethod PaymentMethod { get; set; } // reference to enum PaymentMethodTypes

        private int NumberOfDays
        {
            get
            {
                return (RentalDateTo - RentalDateFrom).Days;
            }
        }

        [Display(Name = "Total Price")]
        [JsonPropertyName("TotalPrice")]
        public double TotalPrice
        {
            get
            {
                // accede a publiv Ilst<Rental_Item_DTO> RentalItems. ri se usa  como variable lamda, no tiene el mismo nombre que la clase en mi caso
                return RentalItems.Sum(ri => ri.PriceForRenting * NumberOfDays*ri.DeviceQuantity );
            }
        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

        public override bool Equals(object? obj)
        {
            return obj is Rental_For_Create dTO &&
                   CompareDate(RentalDateFrom, dTO.RentalDateFrom) &&
                   CompareDate(RentalDateTo, dTO.RentalDateTo) &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   CustomerUserName == dTO.CustomerUserName &&
                   CustomerNameSurname == dTO.CustomerNameSurname &&
                   RentalItems.SequenceEqual(dTO.RentalItems) &&
                   PaymentMethod == dTO.PaymentMethod &&
                   TotalPrice == dTO.TotalPrice;
        }
    }
}

