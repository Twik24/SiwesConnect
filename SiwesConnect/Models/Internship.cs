using System.ComponentModel.DataAnnotations;

namespace SiwesConnect.Models
{
    public class Internship
    {
        public int InternshipID { get; set; }
        public int CompanyID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Duration { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime ApplicationDeadline{ get; set; }
        public string? SupervisorID { get; set; }


    }
}