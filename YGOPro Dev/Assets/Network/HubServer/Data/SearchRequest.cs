namespace DevPro.Network.Data
{
	public class SearchRequest
    {
        public int Format { get; set; }
        public int GameType { get; set; }
        public int BanList { get; set; }
        public int TimeLimit { get; set; }
        public bool ActiveGames { get; set; }
        public bool IlligalGames { get; set; }
        public bool Locked { get; set; }
        public string Filter { get; set; }
    }
}