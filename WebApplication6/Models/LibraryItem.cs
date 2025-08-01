namespace WebApplication6.Models
{
    public class LibraryItem
    {
        public int Id { get; set; }
        public int LibraryId { get; set; }
        public int GameId { get; set; } 
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PhotoUrl { get; set; } 
        public string? CatagoryName { get; set; }
        public Library Library { get; set; }
        public Game Game { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}