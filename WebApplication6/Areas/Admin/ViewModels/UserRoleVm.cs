using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.ViewModels
{
    public class UserRoleVm
    {
        public AppUser AppUser { get; set; } 
        public IEnumerable<string> Roles { get; set; }
    }
}

