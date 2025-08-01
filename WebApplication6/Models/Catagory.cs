

namespace WebApplication6.Models
{
    public class Catagory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GameCatagory> GameCatagories { get; set; } = new List<GameCatagory>();
    }
}
