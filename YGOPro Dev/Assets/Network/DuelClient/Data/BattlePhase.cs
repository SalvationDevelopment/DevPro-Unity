using System.Collections.Generic;

namespace DevPro.Game.Data
{
    public class BattlePhase
    {
        public IList<CardPos> AttackableCards { get; set; }
        public IList<CardPos> ActivableCards { get; set; }
        public IList<int> ActivableDescs { get; set; }
        public bool CanMainPhaseTwo { get; set; }
        public bool CanEndPhase { get; set; }

        public void Init()
        {
            AttackableCards = new List<CardPos>();
            ActivableCards = new List<CardPos>();
            ActivableDescs = new List<int>();
        }
    }
}