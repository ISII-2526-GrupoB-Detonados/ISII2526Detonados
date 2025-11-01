namespace AppForSEII2526.API.DTOs.ReceiptDTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AppForSEII2526.API.Models;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

    public class ReceiptForCreateDTO
    {
        [DataType(DataType.Currency)]
        [Range(0, 100000, ErrorMessage = "El precio total debe de estar entre 0 y 100000")]
        public float TotalPrice { get; set; }

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
    }
       
}
