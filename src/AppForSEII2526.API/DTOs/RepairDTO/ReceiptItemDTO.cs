namespace AppForSEII2526.API.DTOs.RepairDTO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataType = System.ComponentModel.DataAnnotations.DataType;

    public class ReceiptItemDTO
    {
        [Required(ErrorMessage = "El nombre de la reparación es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de la reparación no puede tener más de 50 caracteres o menos de 3.", MinimumLength = 3)]
        public string RepairName { get; set; }

        [Required(ErrorMessage = "La escala es obligatoria.")]
        [StringLength(20, ErrorMessage = "La escala no puede tener más de 20 caracteres o menos de 1.", MinimumLength = 1)]
        public string Scale { get; set; }

        [Required(ErrorMessage = "El modelo a reparar es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del modelo no puede tener más de 50 caracteres o menos de 1.", MinimumLength = 1)]
        public string ModelToRepair { get; set; }

        [Range(10, 10000, ErrorMessage = "El costo de la reparación debe estar entre 10 y 10000.")]
        public float RepairCost { get; set; }

        public ReceiptItemDTO(string repairName, string scale, string modelToRepair, float repairCost)
        {
            RepairName = repairName;
            Scale = scale;
            ModelToRepair = modelToRepair;
            RepairCost = repairCost;
        }
    }
    }