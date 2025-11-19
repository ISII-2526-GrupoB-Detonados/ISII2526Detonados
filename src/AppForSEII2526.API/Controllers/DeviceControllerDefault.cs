using AppForSEII2526.API.DTOs.DevicesDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceControllerDefault : ControllerBase
    {
        //Permite interactuar con la base de datos a través de Entity Framework Core.
        private readonly ApplicationDbContext _context;
        //Variable para escribir logs,  Permite registrar información,
        // errores, advertencias, etc. específicos de DeviceController
        private readonly ILogger<DeviceControllerDefault> _logger; //inyeccion de dependencias
        public DeviceControllerDefault(ApplicationDbContext context, ILogger<DeviceControllerDefault> logger) //constructor 
        {
            _context = context;
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------------------------


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Device_DTO_Alquilar>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevices(string? model, int? maxPrice)
        {
            if (maxPrice < 0)
            {
                return BadRequest("El precio no puede ser negativo ❤️❤️❤️ ");
            }
            var devices = await _context.Devices
                  //-------------------------------------------------------------------------------------------------------------------------
                  //2.1 El sistema permite a los clientes filtrar los dispositivos en función del modelo y/o el precio del alquiler.
                  //poner add para encadernar 2
                .Where(d => d.QuantityForPurchase > 0)
                .Where(d => (model == null || d.Model.NameModel.Contains(model)) &&
                    (priceForRent == null || d.PriceForRent <= priceForRent))


            var devices = await _context.Devices
                // FILTRAR POR DISPONIBILIDAD - Solo dispositivos con stock para alquiler
                .Where(d => d.QuantityForRent > 0)
                // FILTRAR POR MODELO (contiene el texto)
                .Where(d => model == null || d.Model.NameModel.Contains(model))
                // FILTRAR POR PRECIO MÁXIMO
                .Where(d => maxPrice == null || d.PriceForRent <= maxPrice)
                .Select(d => new Device_DTO_Alquilar(
                    d.Id,
                    d.Color,
                    d.Name,
                    d.PriceForRent,
                    d.Year,
                    d.Model.NameModel,
                    d.Brand)
                )
                .ToListAsync();

            return Ok(devices);
        }


    }
}
