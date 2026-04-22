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
            return View(placement);
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
                    return Content("You don't have an active placement yet.");
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
                return Content("You don't have an active placement yet.");
            }

            var entries = _context.LogbookEntries
                .Where(e => e.PlacementID == placement.PlacementID)
                .ToList();

            return View(entries);
        }
    }
}