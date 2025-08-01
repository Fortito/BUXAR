using WebApplication6.Models;

namespace WebApplication6.ViewModels
{
    public class HomeVm
    {
        public Enter LastEnter { get; set; }
        public List<Enter> enters {  get; set; }
        public List<Game> games { get; set; }
        public List<Catagory> catagories { get; set; }
        public List<GameCatagory> gamecatagories { get;set; }
        public List<int> OwnedGameIds { get; set; } 
        public List<Game> TrendingGames { get; set; }
        public List<Game> MostOrderedGames { get; set; }
        public List<Game>? RecommendedGames { get; set; }
        public List<Game> FeaturedGames { get; set; }






    }
}
