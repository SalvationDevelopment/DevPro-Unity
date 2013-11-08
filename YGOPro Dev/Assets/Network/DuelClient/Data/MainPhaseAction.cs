namespace DevPro.Game.Data
{
    public class MainPhaseAction
    {
        public int Action { get; set; }
        public int Index { get; set; }

        public int ToValue()
        {
            return (Index << 16) + Action;
        }
    }
}