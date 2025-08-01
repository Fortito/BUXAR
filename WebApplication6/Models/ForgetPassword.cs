using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class ForgetPassword
    {
        [Required(ErrorMessage = "Write Your Mail Adres")]
        [EmailAddress(ErrorMessage = "Wrong Mail concept")]
        public string Email { get; set; }
    }
}