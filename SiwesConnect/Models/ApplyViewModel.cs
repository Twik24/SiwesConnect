using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class ApplyViewModel
    {
        public int InternshipID { get; set; }
        [Required]
        public string? CoverLetter { get; set; }
    }
}
