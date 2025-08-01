using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class DetailVm
    {
        public Game Game { get; set; }
        public List<Game> RelatedGames { get; set; } = new List<Game>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public double AverageRating { get; set; }
        public int UserRating { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();


    }
}
