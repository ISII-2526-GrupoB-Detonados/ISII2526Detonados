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

    [HttpPost]
    [ProducesResponseType(typeof(ReceiptDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateRepair([FromBody] ReceiptForCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Datos inválidos para crear el recibo");
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
        if (user == null)
        {
            _logger.LogError($"Usuario '{dto.UserName}' no encontrado");
            return BadRequest($"Usuario '{dto.UserName}' no existe");
        }

        var receiptItems = new List<ReceiptItem>();
        foreach (var item in dto.Repairs)
        {
            var repair = await _context.Repairs
                .Include(r => r.Scale)
                .FirstOrDefaultAsync(r => r.Name == item.RepairName && r.Scale.Name == item.Scale);

            if (repair == null)
            {
                _logger.LogError($"Reparación '{item.RepairName}' con escala '{item.Scale}' no encontrada");
                return BadRequest($"Reparación '{item.RepairName}' con escala '{item.Scale}' no existe");
            }

            receiptItems.Add(new ReceiptItem
            {
                Repair = repair,
                Model = item.ModelToRepair
            });
        }

        var receipt = new Receipt
        {
            ApplicationUser = user,
            DeliveryAddress = dto.DeliveryAddress,
            ReceiptDate = DateTime.Now,
            TotalPrice = dto.TotalPrice,
            PaymentMethodTypes = dto.PaymentMethod,
            ReceiptItems = receiptItems
        };

        _context.Receipts.Add(receipt);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRepair), new { id = receipt.Id }, receipt.Id);
    }

}
