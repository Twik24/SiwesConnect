using System.ComponentModel.DataAnnotations;


namespace SiwesConnect.Models
{
    public class CompanyRegisterViewModel
    {
        
        [Required]
        public string? CompanyName { get; set; }
        [Required]
        public string? Industry { get; set; }
        [Required]
        public string? Location { get; set; }
        [Required]
        public string? ContactEmail { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }

    }
}
