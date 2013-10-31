using System.IO;
using System.Text;
using DevPro.Network.Enums;

namespace DevPro.Network
{
    public class MessageReceived
    {
        public DevClientPackets Packet { get; private set; }
        public byte[] Raw { get; private set; }
        public BinaryReader Reader { get; private set; }

        public MessageReceived(DevClientPackets packet, byte[] raw, BinaryReader reader)
        {
            Packet = packet;
            Raw = raw;
            Reader = reader;
        }
		
		public string GetString()
		{
			return Encoding.UTF8.GetString(Raw);	
		}
    }
}