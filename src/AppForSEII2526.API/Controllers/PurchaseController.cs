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
    }
}
