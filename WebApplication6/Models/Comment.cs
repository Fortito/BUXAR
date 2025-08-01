using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace WebApplication6.Models
{

    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int GameId { get; set; }
        public Game Game { get; set; }

        public string UserId { get; set; } 
        public AppUser User { get; set; }
    }
}
