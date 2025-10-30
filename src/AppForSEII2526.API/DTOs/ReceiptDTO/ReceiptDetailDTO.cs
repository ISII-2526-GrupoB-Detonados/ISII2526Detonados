namespace AppForSEII2526.API.DTOs.ReceiptDTO
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

public class ReceiptDetailDTO
{
    [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters or shorter than 2.", MinimumLength = 2)]
    public string Name { get; set; }

    [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters or shorter than 2.", MinimumLength = 2)]
    public string Surname { get; set; }

    [StringLength(100, ErrorMessage = "Delivery address cannot be longer than 100 characters or shorter than 5.", MinimumLength = 5)]
    public string DeliveryAddress { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime OperationDate { get; set; }

    [DataType(DataType.Currency)]
    [Range(0, 100000, ErrorMessage = "Total price must be between 0 and 100000.")]
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
  }
}

