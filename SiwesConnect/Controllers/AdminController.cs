
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var supervisors = await _userManager.GetUsersInRoleAsync("Supervisor");

            ViewBag.TotalStudents = students.Count;
            ViewBag.TotalSupervisors = supervisors.Count;
            ViewBag.TotalInternships = _context.Internships.Count();
            ViewBag.TotalApplications = _context.Applications.Count();
            ViewBag.TotalPlacements = _context.Placements.Count();
            return View();
        }



        public async Task<IActionResult> ViewPlacements()
        {
            var placements = _context.Placements.ToList();
            var result = new List<PlacementDetail>();

            foreach (var placement in placements)
            {
                var student = await _userManager.FindByIdAsync(placement.StudentID!);
                var internship = _context.Internships
                    .FirstOrDefault(i => i.InternshipID == placement.InternshipID);

                result.Add(new PlacementDetail
                {
                    StudentName = student?.FullName ?? "Unknown",
                    InternshipTitle = internship?.Title ?? "Unknown",
                    StartDate = placement.StartDate,
                    EndDate = placement.EndDate,
                });
            }
            ViewBag.Placements = result;
            return View();
        }

    }
}