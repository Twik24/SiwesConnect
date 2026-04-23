using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiwesConnect.Models;
using static SiwesConnect.Controllers.AdminController;

namespace SiwesConnect.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewLogbookEntries()
        {
            var entries = _context.LogbookEntries.ToList();
            return View(entries);
        }

        public async Task<IActionResult> ApproveEntry(int id)
        {
            var entry = _context.LogbookEntries.FirstOrDefault(e => e.EntryId == id);
            if (entry != null)
            {
                entry.ApprovalStatus = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewLogbookEntries");
        }

        public async Task<IActionResult> RejectEntry(int id)
        {
            var entry = _context.LogbookEntries.FirstOrDefault(e => e.EntryId == id);
            if (entry != null)
            {
                entry.ApprovalStatus = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewLogbookEntries");
        }

        public IActionResult AddComment(int id)
        {
            var model = new ReviewViewModel { EntryId = id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(ReviewViewModel model)
        {
            var entry = _context.LogbookEntries.FirstOrDefault(e => e.EntryId == model.EntryId);
            if (entry != null)
            {
                entry.SupervisorComment = model.Comment;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewLogbookEntries");
        }

        public async Task<IActionResult> ViewApplications()
        {
            var applications = _context.Applications.ToList();
            var result = new List<ApplicationDetail>();

            foreach (var app in applications)
            {
                var student = await _userManager.FindByIdAsync(app.StudentID!);
                var internship = _context.Internships.FirstOrDefault(i => i.InternshipID == app.InternshipID);
                result.Add(new ApplicationDetail
                {
                    ApplicationID = app.ApplicationID,
                    StudentName = student?.FullName ?? "Unknown",
                    InternshipTitle = internship?.Title ?? "Unknown",
                    InternshipID = app.InternshipID,
                    ApplicationDate = app.ApplicationDate,
                    CoverLetter = app.CoverLetter,
                    Status = app.Status
                });
            }

            ViewBag.Applications = result;
            return View();
        }

        public async Task<IActionResult> AcceptApplication(int id)
        {
            var application = _context.Applications.FirstOrDefault(a => a.ApplicationID == id);
            if (application != null)
            {
                application.Status = "Accepted";
                var supervisor = await _userManager.GetUserAsync(User);
                var placement = new Placement
                {
                    StudentID = application.StudentID,
                    InternshipID = application.InternshipID,
                    SupervisorID = supervisor!.Id,
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


        public async Task<IActionResult> ViewMyStudents()
        {
            var supervisor = await _userManager.GetUserAsync(User);
            var placements = _context.Placements
                .Where(p => p.SupervisorID == supervisor!.Id)
                .ToList();

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
                    
                    Status = placement.Status
                });
            }

            ViewBag.Placements = result;
            return View();
        }

        public class ApplicationDetail
        {
            public int ApplicationID { get; set; }
            public string? StudentName { get; set; }
            public int InternshipID { get; set; }
            public DateTime ApplicationDate { get; set; }
            public string? Status { get; set; }
            public string? InternshipTitle { get; set; }
            public string? CoverLetter { get; set; }
        }
    }
}