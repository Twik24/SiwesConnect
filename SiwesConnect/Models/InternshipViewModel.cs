using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class InternshipViewModel
    {
       
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
         public string? Description { get; set; }
        [Required]
        public string? Location { get; set; }

        [Required]
        public string? Duration { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ApplicationDeadline { get; set; }
    }
}