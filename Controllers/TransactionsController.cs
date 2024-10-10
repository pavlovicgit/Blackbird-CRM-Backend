using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blackbird_crm.Data;
using blackbird_crm.Models;
using System.Threading.Tasks;
using blackbird_crm.Models.RequestModels.Transactions;
using blackbird_crm.Models.RequestModels.Comments;
using System.Text.Json;

namespace blackbird_crm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] int? projectId = null, [FromQuery] string? searchQuery = null)
        {
            var transactionsQuery = _context.Transactions
                .Include(t => t.Client)
                .Include(t => t.Project)
                .Select(t => new
                {
                    t.Id,
                    t.ClientId,
                    t.ProjectId,
                    t.Amount,
                    t.DueAmount,
                    t.DueDate,
                    t.TransactionStatus,
                    t.Currency,
                    ClientName = t.Client.ClientName,
                    ProjectName = t.Project.ProjectName,
                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                transactionsQuery = transactionsQuery.Where(t =>
                    t.ClientName.ToLower().Contains(searchQuery) ||
                    t.ProjectName.ToLower().Contains(searchQuery)
                );
            }

            if (projectId is not null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.ProjectId == projectId);
            }
            
            var transactions = await transactionsQuery.ToListAsync();

            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Client)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest createTransactionRequest)
        {
            if (createTransactionRequest == null)
            {
                return BadRequest("Request body is null");
            }

            try
            {
                // Log the incoming request
                Console.WriteLine($"Received request: {System.Text.Json.JsonSerializer.Serialize(createTransactionRequest)}");

                var client = await _context.Clients.FindAsync(createTransactionRequest.ClientId);
                if (client == null)
                {
                    return BadRequest($"Client with ID {createTransactionRequest.ClientId} not found");
                }

                var project = await _context.Projects.FindAsync(createTransactionRequest.ProjectId);
                if (project == null)
                {
                    return BadRequest($"Project with ID {createTransactionRequest.ProjectId} not found");
                }

                var transaction = new Transaction
                {
                    ClientId = createTransactionRequest.ClientId,
                    ProjectId = createTransactionRequest.ProjectId,
                    Amount = createTransactionRequest.Amount,
                    DueAmount = createTransactionRequest.DueAmount,
                    DueDate = DateTime.UtcNow,
                    TransactionStatus = createTransactionRequest.TransactionStatus,
                    Currency = createTransactionRequest.Currency
                };

                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Error creating transaction: {ex}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] EditTransactionRequest editTransactionRequest)
        {
            if (id <= 0 || editTransactionRequest == null)
            {
                return BadRequest();
            }

            var existingTransaction = await _context.Transactions.FindAsync(id);
            if (existingTransaction == null)
            {
                return NotFound();
            }

            existingTransaction.Amount = editTransactionRequest.Amount;
            existingTransaction.DueAmount = editTransactionRequest.DueAmount;
            existingTransaction.DueDate = editTransactionRequest.DueDate;
            existingTransaction.TransactionStatus = editTransactionRequest.TransactionStatus;
            existingTransaction.ClientId = editTransactionRequest.ClientId;
            existingTransaction.ProjectId = editTransactionRequest.ProjectId;
            existingTransaction.Currency = editTransactionRequest.Currency;

            var client = await _context.Clients.FindAsync(editTransactionRequest.ClientId);
            if (client != null)
            {
                existingTransaction.Client = client;
            }

            var project = await _context.Projects.FindAsync(editTransactionRequest.ProjectId);
            if (project != null)
            {
                existingTransaction.Project = project;
            }

            _context.Transactions.Update(existingTransaction);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                existingTransaction.Id,
                existingTransaction.ClientId,
                existingTransaction.ProjectId,
                existingTransaction.Amount,
                existingTransaction.DueAmount,
                existingTransaction.DueDate,
                existingTransaction.TransactionStatus,
                existingTransaction.Currency,
                ClientName = existingTransaction.Client.ClientName,
                ProjectName = existingTransaction.Project.ProjectName
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
