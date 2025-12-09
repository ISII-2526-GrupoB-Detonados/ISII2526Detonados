namespace AppForSEII2526.API.DTOs.DevicesDTO
{
    //para alquilar
    public class DeviceDTOAlquilar
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

        public DeviceDTOAlquilar(int id, string color, string name, double priceForRent, int year, string model, string brand)
        {
            Id = id;
            Color = color;
            Name = name;
            PriceForRent = priceForRent;
            Year = year;
            Model = model;
            Brand = brand;
        }
        //Añadido
        public override bool Equals(object obj)
        {
            if (obj is not DeviceDTOAlquilar other)
                return false;

            return Id == other.Id &&
                   Color == other.Color &&
                   Name == other.Name &&
                   PriceForRent.Equals(other.PriceForRent) &&
                   Year == other.Year &&
                   Model == other.Model &&
                   Brand == other.Brand;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Color, Name, PriceForRent, Year, Model, Brand);
        }
    }
}
