namespace prjProgFinalMVC2.ViewModels
{
    public class ClaimViewModel
    {
        public int ClaimId { get; set; }
        public int LecturerId { get; set; }
        public int ModuleId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ModuleName { get; set; }
        public string LecturerName { get; set; }
        public List<DocumentViewModel> Documents { get; set; }

        public DateTime? ReviewDate { get; set; }
        public DateTime? ProcessedDate { get; set; }

        public LecturerViewModel Lecturer { get; set; }

        public string GetLecturerFullName()
        {
            return Lecturer != null
                ? $"{Lecturer.FirstName} {Lecturer.LastName}"
                : "Unknown";
        }
    }

}
