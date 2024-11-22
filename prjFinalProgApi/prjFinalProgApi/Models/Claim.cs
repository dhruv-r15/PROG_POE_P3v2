using System.Reflection.Metadata;

namespace prjFinalProgApi.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public int LecturerId { get; set; }
        public int ModuleId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public virtual Lecturer? Lecturer { get; set; }
        public virtual Module? Module { get; set; }
        public virtual ICollection<Document>? Documents { get; set; }
    }






}
