using System.Collections.Generic;

namespace DevPro.Game.Data
{
	public class SelectedCards
	{
		public IList<CardPos> Cards = new List<CardPos>();
		public bool Cancelable;
	}
}

