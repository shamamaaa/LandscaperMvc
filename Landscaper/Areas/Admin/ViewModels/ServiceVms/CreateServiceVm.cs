using System.ComponentModel.DataAnnotations;

namespace Landscaper.Areas.Admin.ViewModels.ServiceVms
{
    public class CreateServiceVm
    {
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length must be 3")]
        [MaxLength(64, ErrorMessage = "Maximum length must be 64")]
        public string Name { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length must be 3")]
        [MaxLength(320, ErrorMessage = "Maximum length must be 320")]
        public string Description { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
    }
}
