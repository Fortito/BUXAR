using Microsoft.AspNetCore.Identity;

namespace WebApplication6.Models
{
    public class AppUser : IdentityUser
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String? PhotoUrl { get; set; }
    }
}
