namespace AppForSEII2526.API.DTOs.ReceiptDTO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

    public class ReceiptItemDTO
    {
        [Required(ErrorMessage = "El nombre de la reparación es obligatorio.")]
        [StringLength(500, ErrorMessage = "El nombre de la reparación no puede tener más de 500 caracteres o menos de 3.", MinimumLength = 3)]
        public string RepairName { get; set; }

       
        public string? Scale { get; set; }

        [Required(ErrorMessage = "El modelo a reparar es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del modelo no puede tener más de 50 caracteres o menos de 1.", MinimumLength = 1)]
        public string ModelToRepair { get; set; }

        public float? RepairCost { get; set; }

        public ReceiptItemDTO() { }
        public ReceiptItemDTO(string repairName, string scale, string modelToRepair, float repairCost)
        {
            RepairName = repairName;
            Scale = scale;
            ModelToRepair = modelToRepair;
            RepairCost = repairCost;
        }
        public ReceiptItemDTO(string repairName, string modelToRepair)
        {
            RepairName = repairName;
            ModelToRepair = modelToRepair;
        }

    }
   }