using AppForSEII2526.API.DTOs.ReceiptDTO;

[Route("api/[controller]")]
[ApiController]
public class RecibosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RecibosController> _logger;

    public RecibosController(ApplicationDbContext context, ILogger<RecibosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReceiptDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetRepair(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid repair id");
            return BadRequest("Invalid repair id");
        }

        var repair = await _context.Receipts
            .Where(r => r.Id == id)
            .Include(r => r.ApplicationUser)
            .Include(r => r.ReceiptItems)
                .ThenInclude(ri => ri.Repair)
                    .ThenInclude(rep => rep.Scale)
            .Select(r => new ReceiptDetailDTO(
                r.ApplicationUser.Name,
                r.ApplicationUser.Surname,
                r.DeliveryAddress,
                r.ReceiptDate,
                (float)r.TotalPrice,
                r.ReceiptItems.Select(ri => new ReceiptItemDTO(
                    ri.Repair.Name,
                    ri.Repair.Scale.Name,
                    ri.Model,
                    (float)ri.Repair.Cost)).ToList()
            ))
            .FirstOrDefaultAsync();

        if (repair == null)
        {
            _logger.LogError($"Repair with id {id} not found");
            return NotFound();
        }

        return Ok(repair);
    }

}
