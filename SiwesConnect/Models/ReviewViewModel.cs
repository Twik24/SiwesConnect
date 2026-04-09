using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class ReviewViewModel
    {
        public int EntryId { get; set; }
        public string? Comment { get; set; }
    }
}
