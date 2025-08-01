using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }  
    }

}
