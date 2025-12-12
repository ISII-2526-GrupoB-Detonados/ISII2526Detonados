
using AppForSEII2526.API.DTOs.DevicesDTO;
using AppForSEII2526.API.DTOs.Rentals_DTO;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalDeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RentalDeviceController> _logger;
        public RentalDeviceController(ApplicationDbContext context, ILogger<RentalDeviceController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalForCreateDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetRental(int id)
        {
            // Cargar el alquiler incluyendo los dispositivos y modelos relacionados
            var rental = await _context.Rentals
                .Include(r => r.RentDevices)
                    .ThenInclude(rd => rd.Device)
                        .ThenInclude(d => d.Model)
                .Include(r => r.ApplicationUser)
                .Where(r => r.Id == id)
                .Select(r => new RentalForCreateDTO(
                    r.ApplicationUser.Email,                                       // CustomerUserName
                    r.ApplicationUser.Name + " " + r.ApplicationUser.Surname,      // CustomerNameSurname
                    r.DeliveryAddress,
                    r.PaymentMethod,
                    r.RentalDateFrom,
                    r.RentalDateTo,
                    r.RentDevices.Select(rd => new RentalItemDTO(
                        rd.Quantity,                                               // DeviceQuantity
                        rd.Device.Model.NameModel,                                 // DeviceModel
                        rd.Device.Id,                                              // DeviceId
                        rd.Device.Name,                                            // DeviceName
                        rd.Price,                                                  // PriceForRenting
                        rd.Device.Brand                                            // Brand
                    )).ToList()
                ))
                .FirstOrDefaultAsync();

            if (rental == null)
            {
                _logger.LogWarning("Rental with id {RentalId} not found.", id);
                return NotFound();
            }

            return Ok(rental);
        }
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateRental(RentalForCreateDTO rentalForCreate)
        {
            // ========== VALIDACIONES ==========
            if (rentalForCreate.RentalDateFrom <= DateTime.Today)
                ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

            if (rentalForCreate.RentalDateFrom >= rentalForCreate.RentalDateTo)
                ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

            if (rentalForCreate.RentalItems.Count == 0)
                ModelState.AddModelError("RentalItems", "Error! You must include at least one device to be rented");
            //EXAMEN------------------------------------------------------------------------------------------------------------------------------------------------
            if (rentalForCreate.DeliveryAddress == null||(!rentalForCreate.DeliveryAddress.Contains("Calle") && !rentalForCreate.DeliveryAddress.Contains("Carretera")))
            {
                //return BadRequest("Error en la direccion de envio. Porfavor introduce una direccion valida que incluya las palabras calle o carretera");
                ModelState.AddModelError("DeliveryAddress", "Error en la direccion de envio. Porfavor introduce una direccion valida que incluya las palabras calle o carretera");
            }
            //---------------------------------------------------------------------------------------------------------------------------------------------------------
            // Verificar que el usuario existe
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(au => au.UserName == rentalForCreate.CustomerUserName);

            
            if (user == null)
                ModelState.AddModelError("RentalApplicationUser", "Error! UserName is not registered");

            if (ModelState.ErrorCount > 0)
            {
                // AÑADE ESTE LOG AQUÍ TAMBIÉN
                _logger.LogWarning("==== ERRORES DE VALIDACIÓN (Primera verificación) ====");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogWarning($"Campo: {error.Key} - Error: {err.ErrorMessage}");
                    }
                }
                _logger.LogWarning("================================");

                return BadRequest(new ValidationProblemDetails(ModelState));
            }


            // ========== OBTENER LOS IDs DE DISPOSITIVOS ==========
            var deviceIds = rentalForCreate.RentalItems.Select(ri => ri.DeviceId).ToList();

            // ========== CONSULTAR DISPOSITIVOS CON SUS ALQUILERES ==========
            var devices = await _context.Devices
                .Include(d => d.Model)
                .Include(d => d.RentedDevices)
                    .ThenInclude(rd => rd.Rental)
                .Where(d => deviceIds.Contains(d.Id))
                .Select(d => new {
                    d.Id,
                    d.Name,
                    d.Brand,
                    d.QuantityForRent,
                    d.PriceForRent,
                    d.Model.NameModel,
                    // Contar dispositivos alquilados en el período
                    NumberOfRentedDevices = d.RentedDevices
                        .Count(rd => rd.Rental.RentalDateFrom <= rentalForCreate.RentalDateTo
                                && rd.Rental.RentalDateTo >= rentalForCreate.RentalDateFrom)
                })
                .ToListAsync();

            // ========== CREAR EL RENTAL ==========
            Rental rental = new Rental
            {
                ApplicationUser = user,
                DeliveryAddress = rentalForCreate.DeliveryAddress,
                PaymentMethod = rentalForCreate.PaymentMethod,
                RentalDate = DateTime.UtcNow,
                RentalDateFrom = rentalForCreate.RentalDateFrom,
                RentalDateTo = rentalForCreate.RentalDateTo,
                RentDevices = new List<RentDevice>()
            };

            rental.TotalPrice = 0;
            var numDays = (rental.RentalDateTo - rental.RentalDateFrom).TotalDays;

            // ========== PROCESAR CADA ITEM ==========
            foreach (var item in rentalForCreate.RentalItems)
            {
                var device = devices.FirstOrDefault(d => d.Id == item.DeviceId);

                if (device == null)
                {
                    ModelState.AddModelError("RentalItems",
                        $"Error! Device with ID '{item.DeviceId}' does not exist.");
                    continue; // ← CLAVE: Salta al siguiente item sin procesar este
                }
                if (device.QuantityForRent < item.DeviceQuantity)
                {
                    ModelState.AddModelError("RentalItems",
                        $"Error! Not enough stock for '{device.Name}'. Available: {device.QuantityForRent}, Requested: {item.DeviceQuantity}");
                    continue; // ← No procesar este dispositivo
                }
                
                
                
                    // Agregar RentDevice
                    rental.RentDevices.Add(new RentDevice
                    {
                        DeviceId = device.Id,
                        Rental = rental,
                        Price = device.PriceForRent,
                        Quantity = item.DeviceQuantity
                    });
                    item.PriceForRenting = device.PriceForRent;
                    // actualizar stock
                    var deviceEntity = await _context.Devices.FindAsync(device.Id);
                    if (deviceEntity != null)
                    {
                        deviceEntity.QuantityForRent -= item.DeviceQuantity;
                        _logger.LogInformation($"Stock actualizado para '{deviceEntity.Name}': {deviceEntity.QuantityForRent + item.DeviceQuantity} -> {deviceEntity.QuantityForRent}");
                    }
                
            }

            rental.TotalPrice = rental.RentDevices.Sum(rd => rd.Price * rd.Quantity * numDays);

            // Si hay problemas de disponibilidad
            if (ModelState.ErrorCount > 0)
            {
                _logger.LogWarning("==== ERRORES DE VALIDACIÓN ====");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        _logger.LogWarning($"Campo: {error.Key} - Error: {err.ErrorMessage}");
                    }
                }
                _logger.LogWarning("================================");

                // log para  Ver el JSON que se enviará al cliente
                var validationProblem = new ValidationProblemDetails(ModelState);
                var json = System.Text.Json.JsonSerializer.Serialize(validationProblem);
                _logger.LogWarning($"JSON enviado al cliente: {json}");

                return BadRequest(validationProblem);
            }

            _context.Add(rental);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log completo del error
                _logger.LogError(ex, "Error completo al guardar rental");

                // Obtener el mensaje completo incluyendo inner exceptions
                var errorMessage = ex.Message;
                var innerEx = ex.InnerException;

                while (innerEx != null)
                {
                    errorMessage += " | Inner: " + innerEx.Message;
                    innerEx = innerEx.InnerException;
                }

                _logger.LogError(errorMessage);
                return Conflict("Error: " + errorMessage);
            }

            // Retornar RentalDetailDTO
            var rentalDetail = new RentalDetailDTO(
                rental.Id,
                rental.RentalDate,
                rental.ApplicationUser.UserName,
                rentalForCreate.CustomerNameSurname,
                rental.DeliveryAddress,
                rentalForCreate.PaymentMethod,
                rental.RentalDateFrom,
                rental.RentalDateTo,
                rentalForCreate.RentalItems
            );

            return CreatedAtAction("GetRental", new { id = rental.Id }, rentalDetail);
        }
    }
}