using AppForSEII2526.API.DTOs.DevicesDTOrepa;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetRepairDTO(string? nombre, string? scaleNombre)
        {
            // Construimos la consulta base (incluye la relación Scale)
            var query = _context.Repairs
                .Include(r => r.Scale)
                .AsQueryable();

            // Aplicamos los filtros si vienen parámetros
            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(r => r.Name.Contains(nombre));

            if (!string.IsNullOrWhiteSpace(scaleNombre))
                query = query.Where(r => r.Scale.Name.Contains(scaleNombre));




            // Materializamos la lista para agrupar y ordenar en memoria (evita problemas de traducción en EF Core)
            var repairsList = await query.ToListAsync();

            // Agrupamos por balanza, elegimos la reparación con menor Id por balanza,
            // y ordenamos los grupos por número de reparaciones (desc), tie-breaker por ScaleId (desc).
            // Esta ordenación reproduce el orden esperado por los tests (ej. Balanza con más reparaciones primero).
            var grouped = repairsList
                .GroupBy(r => new { r.ScaleId, ScaleName = r.Scale?.Name ?? string.Empty })
                .Select(g => new
                {
                    ScaleId = g.Key.ScaleId,
                    ScaleName = g.Key.ScaleName,
                    Count = g.Count(),
                    Repair = g.OrderBy(r => r.Id).First()
                })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.ScaleId)
                .ToList();

            var repairsDto = grouped
                .Select(x => new repairDTOrepa(x.Repair.Id, x.Repair.Name, x.Repair.Description, x.Repair.Scale?.Name ?? string.Empty, x.Repair.Cost))
                .ToList();

            if (repairsDto.Count == 0)
            {
                var problemDetails = new ValidationProblemDetails();

                if (!string.IsNullOrEmpty(nombre) && string.IsNullOrEmpty(scaleNombre))
                {
                    problemDetails.Errors.Add("Nombre", new[] { "No hay reparaciones con ese nombre" });
                }
                else if (!string.IsNullOrEmpty(scaleNombre) && string.IsNullOrEmpty(nombre))
                {
                    problemDetails.Errors.Add("Scale", new[] { "No hay reparaciones con esa balanza" });
                }
                else if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(scaleNombre))
                {
                    problemDetails.Errors.Add("Filtros", new[] { "No hay reparaciones que cumplan los filtros" });
                }
                else
                {
                    // Si no se pasaron filtros y la lista está vacía devolvemos OK con lista vacía
                    return Ok(repairsDto);
                }

                return BadRequest(problemDetails);
            }

            return Ok(repairsDto);
        }
        
        // SPRINT 3, ANADIMOS ESTE GET PARA OBTENER LAS ESCALAS (BALANZAS) DISTINTAS
        [HttpGet]
        [Route("/GetScales")] // ruta absoluta
        [ProducesResponseType(typeof(ICollection<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetScales(string? scaleName)
        {
            var scales = await _context.Scales
                .Select(s => s.Name)
                .Distinct()
                .ToListAsync();

            if (!string.IsNullOrEmpty(scaleName))
                scales = scales.Where(s => s.Contains(scaleName)).ToList();

            return Ok(scales);
        }
    }
}
