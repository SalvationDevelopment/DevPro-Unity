using DevPro.Network.Enums;

namespace DevPro.Network.Data
{
	public class ChatMessage
    {
        public string message { get; set; }
        public string channel { get; set; }
        public UserData from { get; set; }
        public int type { get; set; }
        public int command { get; set; }
    }
}
