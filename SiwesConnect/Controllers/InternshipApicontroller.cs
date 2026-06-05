using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class InternshipsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InternshipsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/InternshipsApi
        [HttpGet]
        public IActionResult GetAll()
        {
            var internships = _context.Internships.ToList();
            return Ok(internships);
        }

        // GET: api/InternshipsApi/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var internship = _context.Internships.FirstOrDefault(i => i.InternshipID == id);
            if (internship == null)
                return NotFound();
            return Ok(internship);
        }
    }
}