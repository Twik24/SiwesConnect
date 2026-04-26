using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.StudentName = user?.FullName ?? "Student";
            return View();
        }

        public async Task<IActionResult> MyPlacement()
        {
            var user = await _userManager.GetUserAsync(User);
            var placement = _context.Placements
                .FirstOrDefault(p => p.StudentID == user!.Id);

            if (placement == null)
            {
                return RedirectToAction("NoPlacement");
            }

            var internship = _context.Internships
                .FirstOrDefault(i => i.InternshipID == placement.InternshipID);

            ViewBag.InternshipTitle = internship?.Title ?? "Unknown";
            ViewBag.InternshipLocation = internship?.Location ?? "Unknown";
            ViewBag.Duration = internship?.Duration ?? "Unknown";
            ViewBag.StartDate = placement.StartDate;
            ViewBag.EndDate = placement.EndDate;
            ViewBag.Status = placement.Status;

            return View();
        }

        public IActionResult SubmitLogbook()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLogbook(LogbookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var placement = _context.Placements
                    .FirstOrDefault(p => p.StudentID == user!.Id);

                if (placement == null)
                {
                    return RedirectToAction("NoPlacement");
                }

                var existingEntry = _context.LogbookEntries
                 .FirstOrDefault(e => e.PlacementID == placement.PlacementID
                 && e.WeekNumber == model.WeekNumber
                 && e.ApprovalStatus != "Rejected");

                if (existingEntry != null)
                {
                    ModelState.AddModelError("", "You have already submitted an entry for this week. Please wait for your supervisor to review it.");
                    return View(model);
                }

                var entry = new LogbookEntry
                {
                    PlacementID = placement.PlacementID,
                    WeekNumber = model.WeekNumber,
                    WorkDescription = model.WorkDescription,
                    DateSubmitted = DateTime.Now,
                    ApprovalStatus = "Pending"
                };

                _context.LogbookEntries.Add(entry);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyLogbook");
            }

            return View(model);
        }

        public async Task<IActionResult> MyLogbook()
        {
            var user = await _userManager.GetUserAsync(User);
            var placement = _context.Placements
                .FirstOrDefault(p => p.StudentID == user!.Id);

            if (placement == null)
            {
                return RedirectToAction("NoPlacement");
            }

            var entries = _context.LogbookEntries
                .Where(e => e.PlacementID == placement.PlacementID)
                .ToList();

            return View(entries);
        }

        public async Task<IActionResult> MyApplications()
        {
            var user = await _userManager.GetUserAsync(User);
            var applications = _context.Applications
                .Where(a => a.StudentID == user!.Id)
                .ToList();

            var result = new List<ApplicationStatusDetail>();

            foreach (var app in applications)
            {
                var internship = _context.Internships
                    .FirstOrDefault(i => i.InternshipID == app.InternshipID);

                result.Add(new ApplicationStatusDetail
                {
                    InternshipTitle = internship?.Title ?? "Unknown",
                    ApplicationDate = app.ApplicationDate,
                    Status = app.Status
                });
            }

            ViewBag.Applications = result;
            return View();
        }
        public IActionResult NoPlacement()
        {
            return View();
        }
    }
}