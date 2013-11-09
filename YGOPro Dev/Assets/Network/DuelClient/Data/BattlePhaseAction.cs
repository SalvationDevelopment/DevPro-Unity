namespace DevPro.Game.Data
{
    public class BattlePhaseAction
    {
        public int Action { get; set; }
        public int Index { get; set; }
		
        public int ToValue()
        {
            return (Index << 16) + (int)Action;
        }
    }
}