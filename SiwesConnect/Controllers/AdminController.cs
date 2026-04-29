
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
                var supervisor = await _userManager.FindByIdAsync(internship?.SupervisorID ?? "");
                var company = supervisor?.CompanyID != null
                    ? _context.Companies.FirstOrDefault(c => c.CompanyID == supervisor.CompanyID)
                    : null;

                result.Add(new PlacementDetail
                {
                    StudentName = student?.FullName ?? "Unknown",
                    InternshipTitle = internship?.Title ?? "Unknown",
                    CompanyName = company?.CompanyName ?? "Unknown",
                    StartDate = placement.StartDate,
                    EndDate = placement.EndDate,
                    Status = placement.Status
                });
            }

            ViewBag.Placements = result;
            return View();
        }
        public IActionResult ManageCompanies()
        {
            var companies = _context.Companies.ToList();
            ViewBag.Companies = companies;
            return View();
        }

        public async Task<IActionResult> ActivateCompany(int id)
        {
            var company = _context.Companies.FirstOrDefault(c => c.CompanyID == id);
            if (company != null)
            {
                company.Status = "Active";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageCompanies");
        }

        public async Task<IActionResult> RejectCompany(int id)
        {
            var company = _context.Companies.FirstOrDefault(c => c.CompanyID == id);
            if (company != null)
            {
                company.Status = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageCompanies");
        }
    }
}