using System.Collections.Generic;

namespace DevPro.Game.Data
{
    public class BattlePhase
    {
        public IList<CardData> AttackableCards { get; set; }
        public IList<CardData> ActivableCards { get; set; }
        public IList<int> ActivableDescs { get; set; }
        public bool CanMainPhaseTwo { get; set; }
        public bool CanEndPhase { get; set; }

        public void Init()
        {
            AttackableCards = new List<CardData>();
            ActivableCards = new List<CardData>();
            ActivableDescs = new List<int>();
        }
    }
}