using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string? PhotoUrl { get; set; }
        public string Dimensionality { get; set; }
        public decimal Price { get; set; }
        public int  Discount { get; set; }
        public List<GameCatagory> GameCatagories { get; set; } = new List<GameCatagory>();
        [NotMapped]
        public List<int> SelectedCatagoryIds { get; set; } = new List<int>();
        public int ClickCount { get; set; }
        public int ViewCount { get; set; }
        public string? GameUrl { get; set; }
        public ICollection<GameImage> GameImages { get; set; } = new List<GameImage>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
