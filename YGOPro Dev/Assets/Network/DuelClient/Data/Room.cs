namespace DevPro.Game.Data
{
    public class Room
    {
        public bool IsHost { get; set; }
        public string[] Names { get; set; }
        public bool[] IsReady { get; set; }

        public Room()
        {
            Names = new string[8];
            IsReady = new bool[8];
        }
		
		public bool Ready(string username)
		{
			for(int i = 0; i < Names.Length; i++)
			{
				if(Names[i] == username)
					return IsReady[i];
			}
			return false;
		}
    }
}