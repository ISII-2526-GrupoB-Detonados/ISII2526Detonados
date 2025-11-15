
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.DTOs.DevicesDTOrepa
{
    public class repairDTOrepa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El identificador de la balanza es obligatorio.")]
        public string Scale { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Costo")]
        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El costo debe ser positivo.")]
        public double Cost { get; set; }

        // Constructor 
        public repairDTOrepa(int id, string nombre, string descripcion, string escala, double coste)
        {
            Id = id; 
            Name = nombre;
            Description = descripcion;
            Scale = escala;
            Cost = coste;
        }

    }

}
