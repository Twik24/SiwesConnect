using Microsoft.AspNetCore.Identity;

namespace SiwesConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? UserType { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int? CompanyID { get; set; }
    }
}