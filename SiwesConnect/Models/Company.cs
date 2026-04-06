using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string? UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? ContactEmail { get; set; }
        public string? PhoneNumber { get; set; }

     
    }
}