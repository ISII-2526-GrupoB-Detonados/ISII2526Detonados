namespace AppForSEII2526.API.DTOs.Devices_DTO_Comprar_J
{
    public class Device_DTO_Comprar //mirar más y rquired y key
    {
        public Device_DTO_Comprar(int id, string brand, string color, string name, string model, double priceForPurchase)
        {
            Id = id;
            Brand = brand;
            Color = color;
            Name = name;
            Model = model;
            PriceForPurchase = priceForPurchase;
        }


        //Atributos
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public double PriceForPurchase { get; set; }

    }
}
