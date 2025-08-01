namespace WebApplication6.Models
{
    public class Library
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? UserId { get; set; }  
        public AppUser? User { get; set; }
        public List<LibraryItem> Items { get; set; } = new();
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
