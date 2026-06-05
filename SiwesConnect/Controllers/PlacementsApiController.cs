using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiwesConnect.Models;
using System.Security.Claims;

namespace SiwesConnect.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlacementsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlacementsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PlacementsApi
        // Admin — see all placements
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var placements = await _context.Placements.ToListAsync();
            return Ok(placements);
        }

        // GET: api/PlacementsApi/my
        // Student — see their own placement
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyPlacement()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var placement = await _context.Placements
                .FirstOrDefaultAsync(p => p.StudentID == userId);

            if (placement == null)
                return NotFound(new { message = "No placement found" });

            return Ok(placement);
        }

        // GET: api/PlacementsApi/my-students
        // Supervisor — see all students assigned to them
        [HttpGet("my-students")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetMyStudents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var placements = await _context.Placements
                .Where(p => p.SupervisorID == userId)
                .ToListAsync();

            return Ok(placements);
        }

        // POST: api/PlacementsApi
        // Admin — create a placement
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlacement([FromBody] CreatePlacementDto dto)
        {
            var placement = new Placement
            {
                StudentID = dto.StudentID,
                CompanyID = dto.CompanyID,
                InternshipID = dto.InternshipID,
                SupervisorID = dto.SupervisorID,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Active"
            };

            _context.Placements.Add(placement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Placement created successfully" });
        }

        // PUT: api/PlacementsApi/5/status
        // Admin — update placement status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePlacementStatusDto dto)
        {
            var placement = await _context.Placements.FindAsync(id);
            if (placement == null)
                return NotFound(new { message = "Placement not found" });

            placement.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Placement status updated" });
        }
    }

    public class CreatePlacementDto
    {
        public string? StudentID { get; set; }
        public int CompanyID { get; set; }
        public int InternshipID { get; set; }
        public string? SupervisorID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class UpdatePlacementStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}