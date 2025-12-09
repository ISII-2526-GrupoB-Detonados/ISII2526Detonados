

namespace AppForSEII2526.API.DTOs.Purchase_DTO
{
    public class Purchase_Item_DTO
    {
        

        public Purchase_Item_DTO(int deviceID,string name, double price, string brand, string color, string model, int quantity, string description)
        {
            DeviceID = deviceID;
            Name = name;
            Price = price;
            Brand = brand;
            Color = color;
            Model = model;
            Quantity = quantity;
            Description = description;
        }

        public int DeviceID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Model { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Purchase_Item_DTO dTO &&
                   DeviceID == dTO.DeviceID &&
                   Name == dTO.Name &&
                   Price == dTO.Price &&
                   Brand == dTO.Brand &&
                   Color == dTO.Color &&
                   Model == dTO.Model &&
                   Quantity == dTO.Quantity &&
                   Description == dTO.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceID, Name, Price, Brand, Color, Model, Quantity, Description);
        }
    }
}
