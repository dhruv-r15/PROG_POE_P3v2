using System.ComponentModel.DataAnnotations;

namespace prjProgFinalMVC2.ViewModels
{
    public class CreateModuleViewModel
    {
        [Required]
        public string ModuleCode { get; set; }

        [Required]
        public string ModuleName { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal HourlyRate { get; set; }
    }
}
