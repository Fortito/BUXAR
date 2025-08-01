namespace WebApplication6.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        public List<OrderItem> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Library Library { get; set; }
    }
}
