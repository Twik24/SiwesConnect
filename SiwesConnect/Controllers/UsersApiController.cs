using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/UsersApi/students
        // Admin — get all students
        [HttpGet("students")]
        public IActionResult GetStudents()
        {
            var students = _userManager.Users
                .Where(u => u.UserType == "Student")
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.UserType
                })
                .ToList();

            return Ok(students);
        }

        // GET: api/UsersApi/supervisors
        // Admin — get all supervisors
        [HttpGet("supervisors")]
        public IActionResult GetSupervisors()
        {
            var supervisors = _userManager.Users
                .Where(u => u.UserType == "Supervisor")
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.UserType,
                    u.CompanyID
                })
                .ToList();

            return Ok(supervisors);
        }

        // GET: api/UsersApi/{id}
        // Admin — get a specific user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.UserType,
                user.CompanyID
            });
        }
    }
}