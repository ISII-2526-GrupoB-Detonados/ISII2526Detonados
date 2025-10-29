namespace AppForSEII2526.API.DTOs.Devices_DTO_Comprar_J
{
    using DataType = System.ComponentModel.DataAnnotations.DataType;


    public class Device_DTO_Comprar //mirar más y rquired y key
    {

        public Device_DTO_Comprar() { } //evita colapso framework
        public Device_DTO_Comprar(int id, string brand, string color, string name, string model, double priceForPurchase)
        {
            Id = id;
            Brand = brand;
            Color = color;
            Name = name;
            Model = model;
            PriceForPurchase = priceForPurchase;
        }


        //Atributos -- Mismas validaciones que en el modelo Device.cs 

        public int Id { get; set; }
        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(50, ErrorMessage = "La marca no puede superar los 50 caracteres.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "El color es obligatorio.")]
        [StringLength(30, ErrorMessage = "El color no puede superar los 30 caracteres.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El modelo es obligatorio.")]
        public string Model { get; set; }
        
        [DataType(DataType.Currency)]
        [Display(Name = "Precio de alquiler")]
        [Precision(10, 2)]
        [Required(ErrorMessage = "El precio de alquiler es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio de alquiler debe ser positivo.")]
        public double PriceForPurchase { get; set; }

    }
}
