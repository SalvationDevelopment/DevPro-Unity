using System.Collections.Generic;

namespace DevPro.Game.Data
{
    public class MainPhase
    {
        public IList<CardData> SummonableCards { get; private set; }
        public IList<CardData> SpecialSummonableCards { get; private set; }
        public IList<CardData> ReposableCards { get; private set; }
        public IList<CardData> MonsterSetableCards { get; private set; }
        public IList<CardData> SpellSetableCards { get; private set; }
        public IList<CardData> ActivableCards { get; private set; }
        public IList<int> ActivableDescs { get; private set; }
        public bool CanBattlePhase { get; set; }
        public bool CanEndPhase { get; set; }

        public MainPhase()
        {
            SummonableCards = new List<CardData>();
            SpecialSummonableCards = new List<CardData>();
            ReposableCards = new List<CardData>();
            MonsterSetableCards = new List<CardData>();
            SpellSetableCards = new List<CardData>();
            ActivableCards = new List<CardData>();
            ActivableDescs = new List<int>();
        }
    }
}