namespace DevPro.Game.Data
{
    public class Room
    {
        public bool IsHost { get; set; }
        public string[] Names { get; set; }
        public bool[] IsReady { get; set; }
		public bool IsFirst { get; set; }

        public Room()
        {
            Names = new string[8];
            IsReady = new bool[8];
        }
    }
}