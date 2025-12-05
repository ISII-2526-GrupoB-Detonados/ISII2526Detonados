using AppForSEII2526.API.DTOs.ReceiptDTO;

namespace AppForSEII2526.Web
{
    public class ReceiptStateContainer
    {
        public ReceiptForCreateDTO Receipt { get; private set; } = new ReceiptForCreateDTO()
        {
            Repairs = new List<ReceiptItemDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddRepairToReceipt(ReceiptItemDTO repairItem)
        {
            if (!Receipt.Repairs.Any(ri => ri.RepairName == repairItem.RepairName && ri.ModelToRepair == repairItem.ModelToRepair))
            {
                Receipt.Repairs.Add(new ReceiptItemDTO()
                {
                    RepairName = repairItem.RepairName,
                    Scale = repairItem.Scale,
                    ModelToRepair = repairItem.ModelToRepair,
                    RepairCost = repairItem.RepairCost
                });
            }
            NotifyStateChanged();
        }

        public void RemoveRepairItem(ReceiptItemDTO item)
        {
            Receipt.Repairs.Remove(item);
            NotifyStateChanged();
        }

        public void ClearReceiptCart()
        {
            Receipt.Repairs.Clear();
            NotifyStateChanged();
        }

        public void ReceiptProcessed()
        {
            // Hemos completado el proceso de recibo, creamos un nuevo objeto sin datos
            Receipt = new ReceiptForCreateDTO()
            {
                Repairs = new List<ReceiptItemDTO>()
            };
            NotifyStateChanged();
        }
    }
}