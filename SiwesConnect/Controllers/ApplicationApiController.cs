using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiwesConnect.Models;
using System.Security.Claims;

namespace SiwesConnect.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationsApiController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/ApplicationsApi
        // Admin only — see all applications
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var applications = await _context.Applications.ToListAsync();
            return Ok(applications);
        }

        // GET: api/ApplicationsApi/my
        // Student — see their own applications
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applications = await _context.Applications
                .Where(a => a.StudentID == userId)
                .ToListAsync();

            return Ok(applications);
        }

        // POST: api/ApplicationsApi
        // Student — apply for an internship
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Apply([FromBody] CreateApplicationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var alreadyApplied = await _context.Applications
                .AnyAsync(a => a.StudentID == userId
                            && a.InternshipID == dto.InternshipID);

            if (alreadyApplied)
                return BadRequest(new { message = "You have already applied for this internship" });

            var application = new Application
            {
                StudentID = userId,
                InternshipID = dto.InternshipID,
                ApplicationDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application submitted successfully" });
        }

        // PUT: api/ApplicationsApi/5/status
        // Admin — approve or reject an application
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return NotFound(new { message = "Application not found" });

            application.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Application {dto.Status} successfully" });
        }
    }

    public class CreateApplicationDto
    {
        public int InternshipID { get; set; }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}