using AppForSEII2526.API.DTOs.Devices_DTO_Comprar_J;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceControllerPurchases : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<DeviceControllerPurchases> _logger;

        public DeviceControllerPurchases(ApplicationDbContext context, ILogger<DeviceControllerPurchases> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Device_DTO_Comprar>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevicesComprarDTOs(string? color, string? nombre)
        {
            var devices = await _context.Devices
            .Where(d => d.QuantityForPurchase > 0)
            .Where(d => (color == null || d.Color.Contains(color)) &&
                   (nombre == null || d.Name.Contains(nombre)))
            .Select(d=>new Device_DTO_Comprar(d.Id, d.Brand, d.Color, d.Name, d.Model.NameModel, d.PriceForPurchase))
            .ToListAsync();
            return Ok(devices);
        }
    }
}
