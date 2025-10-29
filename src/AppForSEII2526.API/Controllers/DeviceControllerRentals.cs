using AppForSEII2526.API.DTOs.DevicesDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceControllerRentals : ControllerBase
    {
        //Permite interactuar con la base de datos a través de Entity Framework Core.
        private readonly ApplicationDbContext _context;
        //Variable para escribir logs,  Permite registrar información,
        // errores, advertencias, etc. específicos de DeviceController
        private readonly ILogger<DeviceControllerRentals> _logger;
        public DeviceControllerRentals(ApplicationDbContext context, ILogger<DeviceControllerRentals> logger)
        {
            _context = context;
            _logger = logger;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        [HttpGet]// ← Sin esto, no puedes llamar al método vía HTTP GET
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        {
            //BadRequest si op2 = 0 (división por cero)
            if (op2 == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);
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
            //tres clases

        }

        //-------------------------------------------------------------------------------------------------------------------------
        //Filtros
        //Hacer detalle  caso 7 .
    }
}
