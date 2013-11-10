using System.Collections.Generic;

namespace DevPro.Game.Data
{
    public class MainPhase
    {
        public IList<CardPos> SummonableCards { get; set; }
        public IList<CardPos> SpecialSummonableCards { get; set; }
        public IList<CardPos> ReposableCards { get; set; }
        public IList<CardPos> MonsterSetableCards { get; set; }
        public IList<CardPos> SpellSetableCards { get; set; }
        public IList<CardPos> ActivableCards { get; set; }
        public IList<int> ActivableDescs { get;  set; }
        public bool CanBattlePhase { get; set; }
        public bool CanEndPhase { get; set; }

        public void Init()
        {
            SummonableCards = new List<CardPos>();
            SpecialSummonableCards = new List<CardPos>();
            ReposableCards = new List<CardPos>();
            MonsterSetableCards = new List<CardPos>();
            SpellSetableCards = new List<CardPos>();
            ActivableCards = new List<CardPos>();
            ActivableDescs = new List<int>();
        }
    }
}