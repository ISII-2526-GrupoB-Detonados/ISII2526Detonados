namespace AppForSEII2526.API.DTOs.DevicesDTO
{
    //para alquilar
    public class Device_DTO_Alquilar
    {




        //ATR flujo principal sentencia numero dos

        //[JsonPropertyName]?¿
        public int Id { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public double PriceForRent { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }

        public string Brand { get; set; }
        public Device_DTO_Alquilar(int id, string color, string name, double priceForRent, int year, string model, string brand)
        {
            Id = id;
            Color = color;
            Name = name;
            PriceForRent = priceForRent;
            Year = year;
            Model = model;
            Brand = brand;
        }

        

    }
}
