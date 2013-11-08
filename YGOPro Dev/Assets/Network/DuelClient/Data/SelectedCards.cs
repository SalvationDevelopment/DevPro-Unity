using System.Collections.Generic;

namespace DevPro.Game.Data
{
	public class SelectedCards
	{
		public IList<CardData> Cards = new List<CardData>();
		public bool Cancelable;
	}
}

