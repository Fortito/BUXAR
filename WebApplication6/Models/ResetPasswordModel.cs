using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}