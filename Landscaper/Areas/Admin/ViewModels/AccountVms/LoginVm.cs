using System.ComponentModel.DataAnnotations;

namespace Landscaper.Areas.Admin.ViewModels.AccountVms
{
    public class LoginVm
    {
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length must be 3")]
        [MaxLength(320, ErrorMessage = "Maximum length must be 320")]
        public string UserNameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }


    }
}
