using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class LogbookEntry
    {
        [Key] 
        public int EntryId { get; set; }
        public int PlacementID { get; set; }
        public string? WeekNumber { get; set; }
        public string? WorkDescription { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? SupervisorComment { get; set; }
    }
}
