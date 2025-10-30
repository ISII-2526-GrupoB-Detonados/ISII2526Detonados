
namespace AppForSEII2526.API.DTOs.Rentals_DTO

{
    public class Rental_Detail_DTO : Rental_For_Create
    {
        public Rental_Detail_DTO()
        {
        }
        //constructor con herencia (base) y 2 atr mas
        public Rental_Detail_DTO(int id, DateTime rentalDate, string customerUserName, string customerNameSurname,
            string deliveryAddress, PaymentMethod paymentMethod, DateTime rentalDateFrom, DateTime rentalDateTo, IList<Rental_Item_DTO> rentalItems)
            :base(customerUserName,
                 customerNameSurname,
                 deliveryAddress,
                 paymentMethod,
                 rentalDateFrom, 
                 rentalDateTo,
                 rentalItems){
            // fuera del base añadido arriba
            Id = id;
            RentalDate = rentalDate; 

        }
        //------------------- Additional Attributes ------------------

        public int Id { get; set; }

        public DateTime RentalDate { get; set; }

        //------------------ Overload Equals y GetHashCode ------------------
        public override bool Equals(object? obj)
        {
            return obj is Rental_Detail_DTO dTO &&
                   base.Equals(obj) &&
                   TotalPrice == dTO.TotalPrice &&
                   Id == dTO.Id &&
                   CompareDate(RentalDate, dTO.RentalDate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, RentalDate);
        }

    }
}
