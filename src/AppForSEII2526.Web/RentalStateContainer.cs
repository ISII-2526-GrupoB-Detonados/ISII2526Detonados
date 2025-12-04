using AppForSEII2526.API.DTOs.DevicesDTO;
using AppForSEII2526.Web.API;



namespace AppForSEII2526.Web
{
    public class RentalStateContainer
    {
        // Creamos una instancia de Rental cuando se crea RentalStateContainer
        public RentalForCreateDTO Rental { get; private set; } = new RentalForCreateDTO()
        {
            RentalItems = new List<RentalItemDTO>()
        };

        // Calculamos el precio total de los dispositivos seleccionados para alquilar
        public double TotalPrice
        {
            get
            {
                int numberOfDays = (Rental.RentalDateTo - Rental.RentalDateFrom).Days;
                return Rental.RentalItems.Sum(ri => ri.PriceForRenting * numberOfDays * ri.DeviceQuantity);
            }
        }

        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        // Añadir dispositivo al alquiler
        public void AddDeviceToRental(DeviceDTOAlquilar device)
        {
            // Antes de añadir un dispositivo comprobamos si ya ha sido añadido
            if (!Rental.RentalItems.Any(ri => ri.DeviceId == device.Id))
            {
                // Lo añadimos si no está en la lista
                Rental.RentalItems.Add(new RentalItemDTO()
                {
                    DeviceId = device.Id,
                    DeviceName = device.Name,
                    Brand = device.Brand,
                    DeviceModel = device.Model,
                    PriceForRenting = device.PriceForRent,
                    DeviceQuantity = 1
                });
                NotifyStateChanged();
            }
        }

        // Eliminar dispositivos de la lista de dispositivos seleccionados
        public void RemoveRentalItemToRent(RentalItemDTO item)
        {
            Rental.RentalItems.Remove(item);
            NotifyStateChanged();
        }

        // Eliminamos todos los dispositivos de la lista
        public void ClearRentingCart()
        {
            Rental.RentalItems.Clear();
            NotifyStateChanged();
        }

        // Ya hemos terminado el proceso de alquiler, por lo tanto creamos un nuevo Rental
        public void RentalProcessed()
        {
            // Hemos terminado el proceso de alquiler así que creamos un nuevo objeto sin datos
            Rental = new RentalForCreateDTO()
            {
                RentalItems = new List<RentalItemDTO>()
            };
            NotifyStateChanged();
        }
    }
}