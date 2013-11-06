using System;

namespace DevPro.Game.Data
{
	public class DuelStart
	{
		public int[] LifePoints { get; set; }
		public bool IsFirst { get; set; }
		public int PlayerOneDeckSize { get; set; }
		public int PlayerOneExtraSize { get; set; }
		public int PlayerTwoDeckSize { get; set; }
		public int PlayerTwoExtraSize { get; set; }
	}
}

