using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.DTOs.Purchase_DTO;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(ApplicationDbContext context, ILogger<PurchaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Purchase_Detail_DTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPurchase(int id)
        {
            if (_context.Purchases == null)
            {
                _logger.LogWarning("Error: Purchases table does not exist.");
                return NotFound("");
            }

            var purchase = await _context.Purchases
                .Where(p => p.Id == id)
                    .Include(p => p.PurchaseItems) //join table PurchaseItems
                        .ThenInclude(pi => pi.Device)  //join table Device
                            .ThenInclude(d => d.Model) //join table Model
                    .Include(p => p.ApplicationUser) //join table ApplicationUser
                .Select(p => new Purchase_Detail_DTO(p.Id, p.PurchaseDate, p.ApplicationUser.UserName, p.ApplicationUser.Surname,
                       p.DeliveryAddress, (PaymentMethod)p.PaymentMethod, p.PurchaseItems
                            .Select(pi => new Purchase_Item_DTO(
                                 pi.DeviceId,
                                 pi.Device.Name,
                                 pi.Device.PriceForPurchase,
                                 pi.Device.Brand,
                                 pi.Device.Color,
                                 pi.Device.Model.NameModel,
                                 pi.Quantity,
                                 pi.Description))
                            .ToList<Purchase_Item_DTO>()))
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                _logger.LogError($"Error: Purchase with id {id} does not exist");
                return NotFound();
            }

            return Ok(purchase);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(Purchase_Detail_DTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreatePurchase(Purchase_ForCreate_DTO purchaseForCreate)
        {
            // Validaciones básicas
            if (purchaseForCreate.PurchaseItems.Count == 0)
                ModelState.AddModelError("PurchaseItems", "Error! You must include at least one device to be purchased");

            // Validar que el usuario existe
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.UserName == purchaseForCreate.CustomerUserName);
            if (user == null)
                ModelState.AddModelError("PurchaseApplicationUser", "Error! UserName is not registered");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Obtener las marcas y modelos de los dispositivos desde el DTO
            var deviceBrands = purchaseForCreate.PurchaseItems.Select(pi => pi.Brand).ToList();
            var deviceModels = purchaseForCreate.PurchaseItems.Select(pi => pi.Model).ToList();

            // Consultar los dispositivos disponibles con su stock actual
            var devices = _context.Devices
                .Include(d => d.Model)
                .Include(d => d.PurchaseItems)
                .Where(d => deviceBrands.Contains(d.Brand) && deviceModels.Contains(d.Model.NameModel))
                .Select(d => new
                {
                    d.Id,
                    d.Brand,
                    d.Model.NameModel,
                    d.Color,
                    d.PriceForPurchase,
                    d.QuantityForPurchase,
                    // Contar cuántos de este dispositivo ya están en compras (vendidos)
                    NumberOfPurchasedItems = d.PurchaseItems.Sum(pi => pi.Quantity)
                })
                .ToList();

            // Crear la entidad Purchase
            Purchase purchase = new Purchase
            {
                ApplicationUser = user, // Solo necesitas la relación con ApplicationUser
                DeliveryAddress = purchaseForCreate.DeliveryAddress,
                PurchaseDate = DateTime.Now,
                PaymentMethod = (AppForSEII2526.API.Models.PaymentMethod)purchaseForCreate.PaymentMethod,
                PurchaseItems = new List<PurchaseItem>(),
                TotalPrice = 0,
                TotalQuantity = 0
            };

            // Procesar cada item de la compra
            foreach (var item in purchaseForCreate.PurchaseItems)
            {
                var device = devices.FirstOrDefault(d =>
                    d.Brand == item.Brand &&
                    d.NameModel == item.Model &&
                    d.Color == item.Color);

                // Validar disponibilidad del dispositivo
                if (device == null)
                {
                    ModelState.AddModelError("PurchaseItems", $"Error! Device {item.Brand} {item.Model} {item.Color} does not exist");
                }
                else if (item.Quantity > (device.QuantityForPurchase - device.NumberOfPurchasedItems))
                {
                    ModelState.AddModelError("PurchaseItems", $"Error! Device {device.Brand} {device.NameModel} does not have enough stock. Available: {device.QuantityForPurchase - device.NumberOfPurchasedItems}, Requested: {item.Quantity}");
                }
                else
                {
                    // Crear el PurchaseItem y relacionarlo con la compra
                    var purchaseItem = new PurchaseItem
                    {
                        DeviceId = device.Id,
                        Purchase = purchase,
                        Price = device.PriceForPurchase,
                        Quantity = item.Quantity,
                        Description = item.Description
                    };

                    purchase.PurchaseItems.Add(purchaseItem);

                    // Actualizar el precio en el DTO para la respuesta
                    item.Price = device.PriceForPurchase;
                }
            }

            // Calcular el precio total y cantidad total
            purchase.TotalPrice = purchase.PurchaseItems.Sum(pi => pi.Price * pi.Quantity);
            purchase.TotalQuantity = purchase.PurchaseItems.Sum(pi => pi.Quantity);

            // Si hay errores de disponibilidad, retornar error
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(purchase);

            try
            {
                // Guardar en la base de datos tanto la compra como sus items
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Purchase", $"Error! There was an error while saving your purchase, please try again later");
                return Conflict("Error: " + ex.Message);
            }

            // Crear el DTO de respuesta - usar los datos del user para el DTO
            var purchaseDetail = new Purchase_Detail_DTO(
                purchase.Id,
                purchase.PurchaseDate,
                user.UserName, // Del ApplicationUser
                $"{user.Name} {user.Surname}", // Del ApplicationUser
                purchase.DeliveryAddress,
                purchaseForCreate.PaymentMethod,
                purchaseForCreate.PurchaseItems
            );

            return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchaseDetail);
        }
    }
}
