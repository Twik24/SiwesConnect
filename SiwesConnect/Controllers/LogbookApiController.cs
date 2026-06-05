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
    public class LogbookApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogbookApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LogbookApi/my
        // Student — see their own logbook entries
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyEntries()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the student's placement first
            var placement = await _context.Placements
                .FirstOrDefaultAsync(p => p.StudentID == userId);

            if (placement == null)
                return NotFound(new { message = "No placement found. You must be placed before submitting logbook entries." });

            var entries = await _context.LogbookEntries
                .Where(e => e.PlacementID == placement.PlacementID)
                .ToListAsync();

            return Ok(entries);
        }

        // GET: api/LogbookApi/student/{placementId}
        // Supervisor — see logbook entries for a specific placement
        [HttpGet("student/{placementId}")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> GetStudentEntries(int placementId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Confirm this placement belongs to this supervisor
            var placement = await _context.Placements
                .FirstOrDefaultAsync(p => p.PlacementID == placementId
                                       && p.SupervisorID == userId);

            if (placement == null)
                return Forbid();

            var entries = await _context.LogbookEntries
                .Where(e => e.PlacementID == placementId)
                .ToListAsync();

            return Ok(entries);
        }

        // GET: api/LogbookApi/all
        // Admin — see all logbook entries
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var entries = await _context.LogbookEntries.ToListAsync();
            return Ok(entries);
        }

        // POST: api/LogbookApi
        // Student — submit a new logbook entry
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitEntry([FromBody] CreateLogbookEntryDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var placement = await _context.Placements
                .FirstOrDefaultAsync(p => p.StudentID == userId);

            if (placement == null)
                return NotFound(new { message = "No placement found" });

            var entry = new LogbookEntry
            {
                PlacementID = placement.PlacementID,
                WeekNumber = dto.WeekNumber,
                WorkDescription = dto.WorkDescription,
                DateSubmitted = DateTime.UtcNow,
                ApprovalStatus = "Pending"
            };

            _context.LogbookEntries.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logbook entry submitted successfully" });
        }

        // PUT: api/LogbookApi/5/review
        // Supervisor — approve or reject a logbook entry and add comment
        [HttpPut("{id}/review")]
        [Authorize(Roles = "Supervisor")]
        public async Task<IActionResult> ReviewEntry(int id, [FromBody] ReviewLogbookDto dto)
        {
            var entry = await _context.LogbookEntries.FindAsync(id);
            if (entry == null)
                return NotFound(new { message = "Entry not found" });

            entry.ApprovalStatus = dto.ApprovalStatus; // "Approved" or "Rejected"
            entry.SupervisorComment = dto.SupervisorComment;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Entry {dto.ApprovalStatus}" });
        }
    }

    public class CreateLogbookEntryDto
    {
        public string? WeekNumber { get; set; }
        public string? WorkDescription { get; set; }
    }

    public class ReviewLogbookDto
    {
        public string ApprovalStatus { get; set; } = string.Empty;
        public string? SupervisorComment { get; set; }
    }
}