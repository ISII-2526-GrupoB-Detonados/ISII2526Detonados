namespace AppForSEII2526.API.DTOs.ReceiptDTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

public class ReceiptDetailDTO 
    {
    public string Name { get; set; }

    public string Surname { get; set; }

    public string DeliveryAddress { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime OperationDate { get; set; }

    public float TotalPrice { get; set; }

    public IList<ReceiptItemDTO> Repairs { get; set; }

    public ReceiptDetailDTO(string name, string surname, string deliveryAddress, DateTime operationDate, float totalPrice, IList<ReceiptItemDTO> repairs)
    {
        Name = name;
        Surname = surname;
        DeliveryAddress = deliveryAddress;
        OperationDate = operationDate;
        TotalPrice = totalPrice;
        Repairs = repairs;
    }
        public override bool Equals(object? obj)
        {
            return obj is ReceiptDetailDTO dTO &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   DeliveryAddress == dTO.DeliveryAddress &&
                   CompareDate(OperationDate, dTO.OperationDate) && // Usar CompareDate aquí
                   TotalPrice == dTO.TotalPrice &&
                   Repairs.SequenceEqual(dTO.Repairs); // Usar SequenceEqual en lugar de EqualityComparer
        }
        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, DeliveryAddress, OperationDate, TotalPrice, Repairs);
        }

     

    }
  
}

