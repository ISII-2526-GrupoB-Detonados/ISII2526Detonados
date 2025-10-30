namespace AppForSEII2526.API.DTOs.RepairDTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AppForSEII2526.API.Models;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

    public class ReceiptForCreateDTO
    {
        [DataType(DataType.Currency)]
        [Range(0, 100000, ErrorMessage = "Total price must be between 0 and 100000.")]
        public float TotalPrice { get; set; }

        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters or shorter than 2.", MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters or shorter than 2.", MinimumLength = 2)]
        public string Surname { get; set; }

        public string UserName { get; set; }

        [StringLength(100, ErrorMessage = "Delivery address cannot be longer than 100 characters or shorter than 5.", MinimumLength = 5)]
        public string DeliveryAddress { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public IList<ReceiptItemDTO> Repairs { get; set; }

        public ReceiptForCreateDTO(
            float totalPrice,
            string name,
            string surname,
            string userName,
            string deliveryAddress,
            PaymentMethod paymentMethod,
            IList<ReceiptItemDTO> repairs)
        {
            TotalPrice = totalPrice;
            Name = name ?? throw new ArgumentNullException(nameof(Name));
            Surname = surname ?? throw new ArgumentNullException(nameof(Surname));
            UserName = userName ?? throw new ArgumentNullException(nameof(UserName));
            DeliveryAddress = deliveryAddress ?? throw new ArgumentNullException(nameof(DeliveryAddress));
            PaymentMethod = paymentMethod;
            Repairs = repairs ?? throw new ArgumentNullException(nameof(Repairs));
        }
    }
}
