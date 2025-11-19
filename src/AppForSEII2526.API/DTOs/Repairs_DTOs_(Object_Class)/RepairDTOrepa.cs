using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.DTOs.DevicesDTOrepa
{
    public class repairDTOrepa : IEquatable<repairDTOrepa> // Implementacion de igualdad por valor para que las pruebas puedan comparar listas de DTOs por valor
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

        // Implementacion de igualdad por valor para que las pruebas puedan comparar listas de DTOs por valor, (esto es para GetRepairs-test)
        public override bool Equals(object? obj)
        {
            return Equals(obj as repairDTOrepa);
        }

        public bool Equals(repairDTOrepa? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // Comparar propiedades; usar una pequeña tolerancia para valores de punto flotante

            const double epsilon = 1e-6;
            return Id == other.Id
                && string.Equals(Name, other.Name, StringComparison.Ordinal)
                && string.Equals(Description, other.Description, StringComparison.Ordinal)
                && string.Equals(Scale, other.Scale, StringComparison.Ordinal)
                && Math.Abs(Cost - other.Cost) < epsilon;
        }

        public override int GetHashCode()
        {
            // Redondear el costo para evitar problemas de precisión de punto flotante en el hash code
            var roundedCost = Math.Round(Cost, 6);
            return HashCode.Combine(Id, Name, Description, Scale, roundedCost);
        }

        public static bool operator ==(repairDTOrepa? left, repairDTOrepa? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(repairDTOrepa? left, repairDTOrepa? right)
        {
            return !(left == right);
        }
    }
}
