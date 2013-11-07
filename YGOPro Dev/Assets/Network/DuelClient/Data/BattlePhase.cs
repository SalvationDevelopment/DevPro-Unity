using System.Collections.Generic;

namespace DevPro.Game.Data
{
    public class BattlePhase
    {
        public IList<CardData> AttackableCards { get; private set; }
        public IList<CardData> ActivableCards { get; private set; }
        public IList<int> ActivableDescs { get; private set; }
        public bool CanMainPhaseTwo { get; set; }
        public bool CanEndPhase { get; set; }

        public BattlePhase()
        {
            AttackableCards = new List<CardData>();
            ActivableCards = new List<CardData>();
            ActivableDescs = new List<int>();
        }
    }
}