using blackbird_crm.Data;
using blackbird_crm.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using blackbird_crm.Models.ResponseModels.Clients;

namespace blackbird_crm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetClients([FromQuery] string? searchQuery = null)
        {
            var clientsQuery = _context.Clients
                .Select(c => new
                {
                    c.Id,
                    c.ClientName,
                    c.Status,
                    c.Email,
                    c.PhoneNumber
                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                clientsQuery = clientsQuery.Where(c =>
                    c.ClientName.ToLower().Contains(searchQuery) ||
                    c.Email.ToLower().Contains(searchQuery) ||
                    c.PhoneNumber.ToLower().Contains(searchQuery)
                );
            }

            var clients = await clientsQuery.ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpGet("{id}/Details")]
        public async Task<IActionResult> GetClientDetails(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var clientProjects = await _context.Projects
                .Where(x => x.ClientId == id)
                .Select(x => new ClientDetailsProjectResponse
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ProjectName = x.ProjectName,
                    Status = x.Status,
                    StartDate = x.StartDate
                }).ToListAsync();

            var clientComments = await _context.Comments
                .Where(x => x.ClientId == id)
                .Select(x => new ClientDetailsCommentsResponse
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    CommentText = x.CommentText,
                    CreatedDate = x.CreatedDate
                }).ToListAsync();

            var clientTransactions = await _context.Transactions
                .Where(x => x.ClientId == id)
                .Select(x => new ClientDetailsTransactionsResponse
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    Amount = x.Amount,
                }).ToListAsync();

            return Ok(new ClientDetailsResponse
            {
                Id = client.Id,
                ClientName = client.ClientName,
                Status = client.Status,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                Projects = clientProjects,
                Comments = clientComments,
                Transaction = clientTransactions
            });
        }


        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (client == null)
            {
                return BadRequest();
            }

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id != client.Id || client == null)
            {
                return BadRequest();
            }

            var existingClient = await _context.Clients.FindAsync(id);
            if (existingClient == null)
            {
                return NotFound();
            }

            existingClient.ClientName = client.ClientName;
            existingClient.Status = client.Status;
            existingClient.Email = client.Email;
            existingClient.PhoneNumber = client.PhoneNumber;

            _context.Clients.Update(existingClient);
            await _context.SaveChangesAsync();

            return Ok(existingClient); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
