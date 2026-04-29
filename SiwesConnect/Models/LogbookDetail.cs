namespace SiwesConnect.Models
{
    public class LogbookDetail
    {
        public int EntryId { get; set; }
        public string? StudentName { get; set; }
        public string? WeekNumber { get; set; }
        public string? WorkDescription { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? SupervisorComment { get; set; }
    }
}