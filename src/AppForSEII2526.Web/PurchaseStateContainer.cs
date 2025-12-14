using AppForSEII2526.Web.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AppForSEII2526.Web
{
    public class PurchaseStateContainer
    {
        public Purchase_ForCreate_DTO Purchase { get; private set; } = new Purchase_ForCreate_DTO()
        {
            PurchaseItems = new List<Purchase_Item_DTO>()
        };

        public decimal TotalPrice 
        { 
            get 
            {
                return Convert.ToDecimal(Purchase.PurchaseItems.Sum(pi => pi.Price * pi.Quantity));
            }
        }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddDeviceToPurchase(Device_DTO_Comprar device)
        {
            if(!Purchase.PurchaseItems.Any(pi => pi.DeviceID == device.Id))
                Purchase.PurchaseItems.Add(new Purchase_Item_DTO()
                {
                    DeviceID = device.Id,
                    Name = device.Name,
                    Brand = device.Brand,
                    Model = device.Model,
                    Color = device.Color,
                    Price = device.PriceForPurchase,
                    Quantity = 1,
                    Description = null,
                }
            );
        }

        public void RemoveDeviceFromPurchase(Purchase_Item_DTO item)
        {
            Purchase.PurchaseItems.Remove(item);
        }

        public void ClearPurchaseCart()
        {
            Purchase.PurchaseItems.Clear();
        }

        public void PurchaseProcessed()
        {
            Purchase = new Purchase_ForCreate_DTO()
            {
                PurchaseItems = new List<Purchase_Item_DTO>()
            };
        }
    }
}
