using AppForSEII2526.API.DTOs.DevicesDTOrepa;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReparacionesController> _logger;

        public ReparacionesController(ApplicationDbContext context, ILogger<ReparacionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /*
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        {
            if (op2 == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);
        }
        */

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<repairDTOrepa>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetRepairDTO(string? nombre, string? scaleNombre)
        {
            var repair = await _context.Repairs
                .Include(r => r.Scale) 
                .Where(r => (nombre == null || r.Name.Contains(nombre)) &&
                           (scaleNombre == null || r.Scale.Name.Contains(scaleNombre)))
                .Select(r => new repairDTOrepa(r.Id, r.Name, r.Description, r.Scale.Name, r.Cost))
                .ToListAsync();

            // Si no hay resultados y se pasó un filtro, devolver BadRequest con detalles de validación
            if (!repair.Any())
            {
                if (!string.IsNullOrEmpty(nombre))
                {
                    var pd = new ValidationProblemDetails();
                    pd.Errors.Add("nombre", new[] { "No hay reparaciones con ese nombre" });
                    return BadRequest(pd);
                }

                if (!string.IsNullOrEmpty(scaleNombre))
                {
                    var pd = new ValidationProblemDetails();
                    pd.Errors.Add("scaleNombre", new[] { "No hay reparaciones con esa balanza" });
                    return BadRequest(pd);
                }
            }

            return Ok(repair);
        }


    }
}
