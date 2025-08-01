using System.ComponentModel.DataAnnotations;

namespace WebApplication6.DTOs
{
    public class RegisterDto 
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string RepeatPassword { get; set; }
    }
}
