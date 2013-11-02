using System;

namespace DevPro.Game
{
	public class RoomInfo
	{
		public uint banlist { get; set; }
		public int rule { get; set; }
		public int mode { get; set; }
		public bool priority { get; set; }
		public bool checkdeck { get; set; }
		public bool shuffledeck { get; set; }
		public int startlp { get; set; }
		public int starthand { get; set; }
		public int drawcount { get; set; }
		public short timer { get; set; }
	}
}

