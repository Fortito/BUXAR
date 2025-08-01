
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Enter
    {
        public int Id { get; set; }
        [Required, MaxLength(255), MinLength(42)]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string? PhotoUrl { get; set; }

    }
}
