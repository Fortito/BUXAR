

namespace WebApplication6.Models
{


    public class GameCatagory
    {
        public int GameId { get; set; }
        public Game Game { get; set; }

        public int CatagoryId { get; set; }
        public Catagory Catagory { get; set; }
    }

}
