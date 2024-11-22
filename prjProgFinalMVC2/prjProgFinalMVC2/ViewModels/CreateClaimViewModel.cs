using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace prjProgFinalMVC2.ViewModels
{
    public class CreateClaimViewModel
    {
        [Required]
        [Display(Name = "Module")]
        public int ModuleId { get; set; }

        [Required]
        [Range(0.1, 24)]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Supporting Document")]
        public IFormFile Document { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal HourlyRate { get; set; }
        public string ModuleName { get; set; }
        public int LecturerId { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }

        
    }

}
