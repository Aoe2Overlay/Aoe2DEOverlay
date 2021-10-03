namespace Aoe2DEOverlay
{
    public class Player
    {
        public int Id;
        public int Slot;
        public int Color;
        public string Name = "";
        public string Country = "";
        public string Civ = "";
        public Raiting RM1v1 = new Raiting();
        public Raiting RMTeam = new Raiting();
        public Raiting EW1v1 = new Raiting();
        public Raiting EWTeam = new Raiting();
    }
}