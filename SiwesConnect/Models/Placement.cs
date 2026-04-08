namespace SiwesConnect.Models
{
    public class Placement
    {
        public int PlacementID { get; set; }
        public string? StudentID { get; set; }
        public int CompanyID { get; set; }
        public int InternshipID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
    }
}
