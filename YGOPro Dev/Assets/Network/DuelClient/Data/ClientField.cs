using System.Collections.Generic;
using DevPro.Game.Network.Enums;

namespace DevPro.Game.Data
{
    public class ClientField
    {
        public IList<CardData> Hand { get; private set; }
        public CardData[] MonsterZone { get; private set; }
        public CardData[] SpellZone { get; private set; }
        public IList<CardData> Graveyard { get; private set; }
        public IList<CardData> Banished { get; private set; }
        public IList<CardData> Deck { get; private set; }
        public IList<CardData> ExtraDeck { get; private set; }

        public ClientField()
        {
            Hand = new List<CardData>();
            MonsterZone = new CardData[5];
            SpellZone = new CardData[6];
            Graveyard = new List<CardData>();
            Banished = new List<CardData>();
            Deck = new List<CardData>();
            ExtraDeck = new List<CardData>();
        }

        public void Init(int deck, int extra)
        {
            for (int i = 0; i < deck; ++i)
                Deck.Add(new CardData() { Id = 0, Location = (int)CardLocation.Deck, Position = 0});
            for (int i = 0; i < extra; ++i)
                ExtraDeck.Add(new CardData() { Id = 0, Location = (int)CardLocation.Extra, Position = 0});
        }
    }
}