using System.IO;
using DevPro.Game.Network.Enums;
using DevPro.Game.Network.Helpers;

namespace DevPro.Game.Network.Helpers
{
    public class GameClientPacket
    {
        private BinaryWriter m_writer;
        private MemoryStream m_stream;

        public GameClientPacket(CtosMessage message)
        {
            m_stream = new MemoryStream();
            m_writer = new BinaryWriter(m_stream);
            m_writer.Write((byte)message);
        }

        public byte[] GetContent()
        {
            return m_stream.ToArray();
        }

        public void Write(byte[] array)
        {
            m_writer.Write(array);
        }

        public void Write(sbyte value)
        {
            m_writer.Write(value);
        }

        public void Write(byte value)
        {
            m_writer.Write(value);
        }

        public void Write(short value)
        {
            m_writer.Write(value);
        }

        public void Write(int value)
        {
            m_writer.Write(value);
        }

        public void Write(string text, int len)
        {
            m_writer.WriteUnicode(text, len);
        }
    }
}