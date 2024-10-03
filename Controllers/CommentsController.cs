using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blackbird_crm.Data;
using blackbird_crm.Models;
using System.Threading.Tasks;
using blackbird_crm.Models.RequestModels.Projects;
using blackbird_crm.Models.RequestModels.Comments;

namespace blackbird_crm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] string? searchQuery = null, [FromQuery] string? selectedProjectId = null)
        {
            var commentsQuery = _context.Comments
                .Include(c => c.Client)
                .Include(c => c.Project)
                .Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    c.ProjectId,
                    c.CommentText,
                    ClientName = c.Client.ClientName,
                    ProjectName = c.Project.ProjectName,
                }).AsQueryable();

 
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower(); 
                commentsQuery = commentsQuery.Where(c =>
                    c.CommentText.ToLower().Contains(searchQuery) ||
                    c.ClientName.ToLower().Contains(searchQuery) ||
                    c.ProjectName.ToLower().Contains(searchQuery)
                );
            }

            if (!string.IsNullOrEmpty(selectedProjectId) && int.TryParse(selectedProjectId, out int projectId))
            {
                commentsQuery = commentsQuery.Where(c => c.ProjectId == projectId);
            }

            var comments = await commentsQuery.ToListAsync();

            return Ok(comments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Client)
                .Include(c => c.Project)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetCommentsByProject(int projectId)
        {
            var comments = await _context.Comments
                .Include(c => c.Client)
                .Include(c => c.Project)
                .Where(c => c.ProjectId == projectId)
                .Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    c.ProjectId,
                    c.CommentText,
                    ClientName = c.Client.ClientName,
                    ProjectName = c.Project.ProjectName,
                    c.CreatedDate
                })
                .ToListAsync();

            return Ok(comments);
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest createCommentRequest)
        {
            if (createCommentRequest == null)
            {
                return BadRequest();
            }

            var client = await _context.Clients.FindAsync(createCommentRequest.ClientId);
            if (client == null)
            {
                return BadRequest("Invalid client ID.");
            }

            var project = await _context.Projects.FindAsync(createCommentRequest.ProjectId);
            if (project == null || project.ClientId != createCommentRequest.ClientId)
            {
                return BadRequest("Invalid project ID or the project does not belong to the client.");
            }

            var comment = new Comment
            {
                Client = client,
                Project = project,
                CommentText = createCommentRequest.CommentText,
                CreatedDate = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] EditCommentRequest editCommentRequest)
        {
            if (id <= 0 || editCommentRequest == null)
            {
                return BadRequest();
            }

            var existingComment = await _context.Comments.FindAsync(id);
            if (existingComment == null)
            {
                return NotFound();
            }

            existingComment.CommentText = editCommentRequest.CommentText;
            existingComment.ClientId = editCommentRequest.ClientId;
            existingComment.ProjectId = editCommentRequest.ProjectId;
            

            var client = await _context.Clients.FindAsync(editCommentRequest.ClientId);
            if (client != null)
            {
                existingComment.Client = client;
            }

            var project = await _context.Projects.FindAsync(editCommentRequest.ProjectId);
            if (project != null)
            {
                existingComment.Project = project;
            }

            _context.Comments.Update(existingComment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                existingComment.Id,
                existingComment.ClientId,
                existingComment.ProjectId,
                existingComment.CommentText,
                ClientName = existingComment.Client.ClientName,
                ProjectName = existingComment.Project.ProjectName
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
