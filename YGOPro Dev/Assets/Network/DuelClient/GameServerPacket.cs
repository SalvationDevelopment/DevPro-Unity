using System.IO;
using DevPro.Game.Network.Enums;
using DevPro.Game.Network.Helpers;

namespace DevPro.Game.Network
{
    public class GameServerPacket
    {
        public byte[] Content { get; private set; }

        private BinaryReader m_reader;

        public GameServerPacket(byte[] content)
        {
            Content = content;
            m_reader = new BinaryReader(new MemoryStream(Content));
        }

        public StocMessage ReadStoc()
        {
            return (StocMessage)m_reader.ReadByte();
        }

        public GameMessage ReadGameMsg()
        {
            return (GameMessage)m_reader.ReadByte();
        }

        public byte ReadByte()
        {
            return m_reader.ReadByte();
        }

        public byte[] ReadToEnd()
        {
            return m_reader.ReadBytes((int)m_reader.BaseStream.Length - (int)m_reader.BaseStream.Position);
        }

        public sbyte ReadSByte()
        {
            return m_reader.ReadSByte();
        }

        public short ReadInt16()
        {
            return m_reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return m_reader.ReadInt32();
        }

        public string ReadUnicode(int len)
        {
            return m_reader.ReadUnicode(len);
        }

        public long GetPosition()
        {
            return m_reader.BaseStream.Position;
        }

        public void SetPosition(long pos)
        {
            m_reader.BaseStream.Position = pos;
        }
    }
}