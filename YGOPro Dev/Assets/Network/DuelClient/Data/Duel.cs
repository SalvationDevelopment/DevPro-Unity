using DevPro.Game.Network.Enums;
using DevPro.Game.Data;

namespace DevPro.Game
{
    public class Duel
    {
        public bool IsFirst { get; set; }

        public int[] LifePoints { get; private set; }
        public ClientField[] Fields { get; private set; }

        public int Turn { get; set; }
        public int Player { get; set; }
        public Phase Phase { get; set; }
        public MainPhase MainPhase { get; set; }
        public BattlePhase BattlePhase { get; set; }

        public Duel()
        {
            LifePoints = new int[2];
            Fields = new ClientField[2];
            Fields[0] = new ClientField();
            Fields[1] = new ClientField();
			
        }

        public CardData GetCard(int player, CardLocation loc, int index)
        {
            switch (loc)
            {
                case CardLocation.SpellZone:
                    return Fields[player].SpellZone[index];
                case CardLocation.MonsterZone:
                    return Fields[player].MonsterZone[index];
                case CardLocation.Hand:
                    if (Fields[player].Hand.Count > index)
                        return Fields[player].Hand[index];
                    break;
                case CardLocation.Grave:
                    if (Fields[player].Graveyard.Count > index)
                        return Fields[player].Graveyard[index];
                    break;
                case CardLocation.Removed:
                    if (Fields[player].Banished.Count > index)
                        return Fields[player].Banished[index];
                    break;
                case CardLocation.Deck:
                    if (Fields[player].Deck.Count > index)
                        return Fields[player].Deck[index];
                    break;
                case CardLocation.Extra:
                    if (Fields[player].ExtraDeck.Count > index)
                        return Fields[player].ExtraDeck[index];
                    break;
            }
            return null;
        }

        public void AddCard(CardLocation loc, int cardId, int player, int zone, int pos)
        {
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Add(new CardData() { Id = cardId, Location = (int)loc, Position = pos });
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Add(new CardData() { Id = cardId, Location = (int)loc, Position = pos });
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Add(new CardData() { Id = cardId, Location = (int)loc, Position = pos });
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[zone] = new CardData() { Id = cardId, Location = (int)loc, Position = pos };
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[zone] = new CardData() { Id = cardId, Location = (int)loc, Position = pos };
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Add(new CardData() { Id = cardId, Location = (int)loc, Position = pos });
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Add(new CardData() { Id = cardId, Location = (int)loc, Position = pos });
                    break;
            }
        }

        public void RemoveCard(CardLocation loc, CardData card, int player, int zone)
        {
            switch (loc)
            {
                case CardLocation.Hand:
                    Fields[player].Hand.Remove(card);
                    break;
                case CardLocation.Grave:
                    Fields[player].Graveyard.Remove(card);
                    break;
                case CardLocation.Removed:
                    Fields[player].Banished.Remove(card);
                    break;
                case CardLocation.MonsterZone:
                    Fields[player].MonsterZone[zone] = null;
                    break;
                case CardLocation.SpellZone:
                    Fields[player].SpellZone[zone] = null;
                    break;
                case CardLocation.Deck:
                    Fields[player].Deck.Remove(card);
                    break;
                case CardLocation.Extra:
                    Fields[player].ExtraDeck.Remove(card);
                    break;
            }
        }

        public int GetLocalPlayer(int player)
        {
            return IsFirst ? player : 1 - player;
        }
    }
}