namespace AppForSEII2526.API.DTOs.ReceiptDTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AppForSEII2526.API.Models;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

    public class ReceiptForCreateDTO
    {

        [StringLength(20, ErrorMessage = "El nombre no puede ser mayor de 20 caracteres ni menor de 2", MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "El apellido no puede contener mas de 50 caracteres o menos de 2 caracteres", MinimumLength = 2)]
        public string Surname { get; set; }

        public string UserName { get; set; }

        [StringLength(100, ErrorMessage = "La direccion de envio no debe contener mas de 100 caracteres o menos de 5", MinimumLength = 5)]
        public string DeliveryAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public IList<ReceiptItemDTO> Repairs { get; set; }

        public ReceiptForCreateDTO() { }

        public override bool Equals(object? obj)
        {
            return obj is ReceiptForCreateDTO dTO &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   UserName == dTO.UserName &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   PaymentMethod == dTO.PaymentMethod &&
                   EqualityComparer<IList<ReceiptItemDTO>>.Default.Equals(Repairs, dTO.Repairs);
        }
        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, UserName, DeliveryAddress, PaymentMethod, Repairs);
        }
    }
       
}
