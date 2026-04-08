
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewApplications()
        {
            var applications = _context.Applications.ToList();
            return View(applications);
        }

        public async Task<IActionResult> AcceptApplication(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationID == id);

            if (application != null)
            {
                application.Status = "Accepted";

                var placement = new Placement
                {
                    StudentID = application.StudentID,
                    InternshipID = application.InternshipID,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6),
                    Status = "Active"
                };

                _context.Placements.Add(placement);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewApplications");
        }

        public async Task<IActionResult> RejectApplication(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationID == id);

            if (application != null)
            {
                application.Status = "Rejected";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewApplications");
        }
    }
}