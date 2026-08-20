using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolioBackend.Data;
using MyPortfolioBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;

namespace MyPortfolioBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProjectsController(DataContext context)
        {
            _context = context;
        }

        // 1. GET ALL PROJECTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            AddCorsHeaders();
            try
            {
                return await _context.Projects.Include(p => p.Gallery).ToListAsync();
            }
            catch (Exception)
            {
                return StatusCode(503, "Backend connection failed.");
            }
        }

        // 2. GET SINGLE PROJECT (For the Edit Form)
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            AddCorsHeaders();
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Gallery)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null) return NotFound();
                return project;
            }
            catch (Exception)
            {
                return StatusCode(503, "Backend connection failed.");
            }
        }

        // 3. POST NEW PROJECT
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            AddCorsHeaders();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 4. UPDATE EXISTING PROJECT (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project updatedProject)
        {
            AddCorsHeaders();
            var existingProject = await _context.Projects
                .Include(p => p.Gallery)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProject == null) return NotFound();

            // Update Properties
            existingProject.Title = updatedProject.Title;
            existingProject.Category = updatedProject.Category;
            existingProject.Brand = updatedProject.Brand;
            existingProject.Description = updatedProject.Description;
            existingProject.Location = updatedProject.Location;
            existingProject.Year = updatedProject.Year;
            existingProject.MainImage = updatedProject.MainImage;

            // Update Gallery (Remove old links, add new ones)
            _context.ProjectImages.RemoveRange(existingProject.Gallery);
            existingProject.Gallery = updatedProject.Gallery;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }
        // 5. DELETE PROJECT
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            AddCorsHeaders();
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Handle preflight CORS requests
        [HttpOptions]
        public IActionResult Options()
        {
            AddCorsHeaders();
            Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
            Response.Headers["Access-Control-Allow-Headers"] = "Content-Type,Authorization";
            return Ok();
        }

        private void AddCorsHeaders()
        {
            // Allow any origin — adjust for production as needed
            Response.Headers["Access-Control-Allow-Origin"] = "*";
        }

        private bool ProjectExists(int id) => _context.Projects.Any(e => e.Id == id);
    }
}