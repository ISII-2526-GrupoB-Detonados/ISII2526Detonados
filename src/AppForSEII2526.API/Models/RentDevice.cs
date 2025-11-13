using AppForSEII2526.API.DTOs.Rentals_DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models

{
    //---------------------------------------------------------------------------------------

    // Clave primaria compuesta (DeviceId + RentId) se configura en el DbContext
    [PrimaryKey(nameof(DeviceId),nameof(RentId))]
    public class RentDevice
    {
        //---------------------------------------------------------------------------------------

        // Clave primaria compuesta (DeviceId + RentId) se configura en el DbContext
        [Required(ErrorMessage = "El identificador del dispositivo es obligatorio.")]
        public int DeviceId { get; set; }
    
        
        [Required(ErrorMessage = "El identificador del alquiler es obligatorio.")]
        public int RentId { get; set; }//fractura de la bdd

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser positivo.")]
        [Precision(18, 2)]
        public double Price { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Quantity { get; set; }
        //---------------------------------------------------------------------------------------

        

        public Device Device { get; set; }        // relación con Device
        public Rental Rental { get; set; }        // relación con Rental
        //---------------------------------------------------------------------------------------
        //Constructores
        public RentDevice() { }

        public RentDevice(int deviceId, int rentId, int quantity, double price)
        {
            DeviceId = deviceId;
            RentId = rentId;
            Quantity = quantity;
            Price = price;
        }
        //---------------------------------------------------------------------------------------

        // Metodos overload equals y gethashcode

        // Sobrescribir Equals
        public override bool Equals(object obj)
        {

            return obj is RentDevice rd &&
                   rd.DeviceId == this.DeviceId &&
                   rd.RentId == this.RentId;
        }

        // Sobrescribir GetHashCode
        public override int GetHashCode()
        {
            return (DeviceId, RentId).GetHashCode();
        }

       
    }
}
