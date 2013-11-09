using System.Collections.Generic;

namespace DevPro.Game.Data
{
    public class MainPhase
    {
        public IList<CardData> SummonableCards { get; set; }
        public IList<CardData> SpecialSummonableCards { get; set; }
        public IList<CardData> ReposableCards { get; set; }
        public IList<CardData> MonsterSetableCards { get; set; }
        public IList<CardData> SpellSetableCards { get; set; }
        public IList<CardData> ActivableCards { get; set; }
        public IList<int> ActivableDescs { get;  set; }
        public bool CanBattlePhase { get; set; }
        public bool CanEndPhase { get; set; }

        public void Init()
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