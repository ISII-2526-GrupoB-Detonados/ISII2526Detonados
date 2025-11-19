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

        // [HttpGet]
        //  [Route("[action]")]
        //  [ProducesResponseType(typeof(IList<Device_DTO_Comprar>), (int) HttpStatusCode.OK)]
        //  public async Task<ActionResult> GetDevicesComprarDTOs(string? color, string? nombre)
        //  {
        //     var devices = await _context.Devices
        //      .Where(d => (color == null || d.Color.Contains(color)) &&
        //            (nombre == null || d.Name.Contains(nombre)))
        //      .Select(d=>new Device_DTO_Comprar(d.Id, d.Brand, d.Color, d.Name, d.Model.NameModel, d.PriceForPurchase))
        //      .ToListAsync();
        //      return Ok(devices);
        //  }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Device_DTO_Comprar>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevicesComprarDTOs(string? color, string? nombre)
        {
            // FILTRO POR COLOR — si no existe, BadRequest
            if (color != null)
            {
                bool existsColor = await _context.Devices.AnyAsync(d => d.Color == color && d.QuantityForPurchase > 0);

                if (!existsColor)
                {
                    ModelState.AddModelError("Color", "No hay dispositivos con ese color");
                    _logger.LogError($"{DateTime.Now} Error: No hay dispositivos con ese color ({color})");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
            }

            // FILTRO POR NOMBRE — si no existe, BadRequest
            if (nombre != null)
            {
                bool existsName = await _context.Devices.AnyAsync(d => d.Name.Contains(nombre));

                if (!existsName)
                {
                    ModelState.AddModelError("Nombre", "No hay dispositivos con ese nombre");
                    _logger.LogError($"{DateTime.Now} Error: No hay dispositivos con ese nombre ({nombre})");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
            }

            // Consulta principal
            IList<Device_DTO_Comprar> devices = await _context.Devices

                // Solo dispositivos con stock
                .Where(d => d.QuantityForPurchase > 0)

                .Where(d =>
                       (color == null || d.Color == color)
                    && (nombre == null || d.Name.Contains(nombre))
                )

                .OrderBy(d => d.Name)

                .Select(d => new Device_DTO_Comprar(
                    d.Id,
                    d.Brand,
                    d.Color,
                    d.Name,
                    d.Model.NameModel,
                    d.PriceForPurchase
                ))

                .ToListAsync();

            return Ok(devices);
        }

    }
}
