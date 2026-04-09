using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class LogbookViewModel
    {
        [Required]
        public string? WeekNumber { get; set; }
        public string? WorkDescription { get; set; } 
    }
}
