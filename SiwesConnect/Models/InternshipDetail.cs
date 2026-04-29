namespace SiwesConnect.Models
{
    public class InternshipDetail
    {
        public int InternshipID { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public string? Duration { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public string? Description { get; set; }
        public string? CompanyName { get; set; }
    }
}