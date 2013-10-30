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

        public ChatMessage(MessageType type, CommandType command,UserData user, string channel, string message)
        {
            this.message = message;
            this.type = (int)type;
            this.channel = channel;
            this.command = (int)command;
            this.from = user;
        }
        public ChatMessage(MessageType type,CommandType command, string channel, string message)
        {
            this.message = message;
            this.type = (int)type;
            this.channel = channel;
            this.command = (int)command;
        }
    }
}
