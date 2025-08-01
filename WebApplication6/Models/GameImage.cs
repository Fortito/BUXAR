namespace WebApplication6.Models
{
    public class GameImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } 
        public int GameId { get; set; }
        public Game Game { get; set; }
    }

}
