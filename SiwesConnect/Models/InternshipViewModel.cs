using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class InternshipViewModel
    {
        [Required]
        
        public string? Title { get; set; }

        [Required]
         public string? Description { get; set; }
        [Required]
        public string? Location { get; set; }

        [Required]
        public string? Duration { get; set; }

        [Required]
        public DateTime ApplicationDeadline { get; set; }
    }
}