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


        //Meter los gets de la clase device
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<Device_DTO_Alquilar>), (int)HttpStatusCode.OK)]
        //Devuelve una lista de dispositivos en formato DeviceDTO con un código de estado HTTP 200 (OK) si la operación es exitosa.
        //son X datos donde x es c.Id,c.Color,c.Name,c.PriceForRent,c.Year,c.Model,c.Brand 
        public async Task<ActionResult> GetDevices(string? model, int? priceForRent)
        {

            //❤️❤️❤️ no meter negativos ❤️❤️❤️
            if (priceForRent < 0)
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


               //-------------------------------------------------------------------------------------------------------------------------
               .Select(d => new Device_DTO_Alquilar(
                  d.Id,
                 d.Color,
                 d.Name,
                 d.PriceForRent,
                 d.Year,
                 d.Model.NameModel, //usar un tipo string o Convertir a string para acceder al metodo Contains
                 d.Brand)
               )


                .ToListAsync();
            return Ok(devices);


        }


    }
}