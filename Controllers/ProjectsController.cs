using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using blackbird_crm.Models;
using blackbird_crm.Data;
using blackbird_crm.Models.RequestModels.Projects;

namespace blackbird_crm.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects([FromQuery] int? clientId = null, [FromQuery] string? searchQuery = null)
    {
        var projectsQuery = _context.Projects
            .Include(p => p.Client)
            .Select(p => new
            {
                p.Id,
                p.ProjectName,
                StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                p.Status,
                ClientName = p.Client.ClientName,
                p.ClientId
            }).AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            searchQuery = searchQuery.ToLower();
            projectsQuery = projectsQuery.Where(p =>
                p.ProjectName.ToLower().Contains(searchQuery) ||
                p.ClientName.ToLower().Contains(searchQuery)
            );
        }

        if (clientId is not null)
        {
            projectsQuery = projectsQuery.Where(p => p.ClientId == clientId);
        }

        var projects = await projectsQuery.ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _context.Projects
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (project == null)
        {
            return NotFound();
        }
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest createProjectRequest)
    {
        if (createProjectRequest == null)
        {
            return BadRequest();
        }

        var client = await _context.Clients.FindAsync(createProjectRequest.ClientId);
        if (client == null)
        {
            return BadRequest("Invalid client ID.");
        }

        var project = new Project {
            Client = client,
            ProjectName = createProjectRequest.ProjectName,
            StartDate = DateTime.UtcNow,
            Status = createProjectRequest.Status
        };

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] EditProjectRequest editProjectRequest)
    {
        if (id <= 0 || editProjectRequest == null)
        {
            return BadRequest("Invalid request.");
        }

        var existingProject = await _context.Projects
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingProject == null)
        {
            return NotFound();
        }

        var client = await _context.Clients.FindAsync(editProjectRequest.ClientId);
        if (client == null)
        {
            return BadRequest("Invalid client ID.");
        }

        existingProject.ProjectName = editProjectRequest.ProjectName;
        existingProject.Status = editProjectRequest.Status;
        existingProject.Client = client;


        _context.Projects.Update(existingProject);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            existingProject.Id,
            existingProject.ProjectName,
            existingProject.StartDate,
            existingProject.Status,
            ClientName = existingProject.Client.ClientName,
            existingProject.ClientId
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}