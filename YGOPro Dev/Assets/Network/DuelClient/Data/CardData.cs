using System;
using System.Collections.Generic;
using DevPro.Game.Network.Helpers;
using DevPro.Game.Network.Enums;

namespace DevPro.Game.Data
{
	public class CardData
	{
		public int Id { get; set; }
        public int Position { get; set; }
        public int Location { get; set; }
        public int Alias { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public int Type { get; set; }
        public int Attribute { get; set; }
        public int Race { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefence { get; set; }
        public int Owner { get; set; }
        public int Controller { get; set; }
		
		public int[] ActionIndex { get; set; }
		public IDictionary<int, int> ActionActivateIndex { get; private set; }
		
		public CardData()
		{
			ActionIndex = new int[16];
			ActionActivateIndex = new Dictionary<int, int>();
		}
		
		public void Update(GameServerPacket packet, Duel duel)
        {
            int flag = packet.ReadInt32();
            if ((flag & (int)Query.Code) != 0)
                Id = packet.ReadInt32();
            if ((flag & (int) Query.Position) != 0)
            {
                Controller = duel.GetLocalPlayer(packet.ReadByte());
                packet.ReadByte();
                packet.ReadByte();
                Position = packet.ReadByte();
            }
            if ((flag & (int)Query.Alias) != 0)
                Alias = packet.ReadInt32();
            if ((flag & (int)Query.Type) != 0)
                Type = packet.ReadInt32();
            if ((flag & (int)Query.Level) != 0)
                Level = packet.ReadInt32();
            if ((flag & (int)Query.Rank) != 0)
                Rank = packet.ReadInt32();
            if ((flag & (int)Query.Attribute) != 0)
                Attribute = packet.ReadInt32();
            if ((flag & (int)Query.Race) != 0)
                Race = packet.ReadInt32();
            if ((flag & (int)Query.Attack) != 0)
                Attack = packet.ReadInt32();
            if ((flag & (int)Query.Defence) != 0)
                Defense = packet.ReadInt32();
            if ((flag & (int)Query.BaseAttack) != 0)
                BaseAttack = packet.ReadInt32();
            if ((flag & (int)Query.BaseDefence) != 0)
                BaseDefence = packet.ReadInt32();
            if ((flag & (int)Query.Reason) != 0)
                packet.ReadInt32();
            if ((flag & (int)Query.ReasonCard) != 0)
                packet.ReadInt32(); // Int8 * 4
            if ((flag & (int)Query.EquipCard) != 0)
                packet.ReadInt32(); // Int8 * 4
            if ((flag & (int)Query.TargetCard) != 0)
            {
                int count = packet.ReadInt32();
                for (int i = 0; i < count; ++i)
                    packet.ReadInt32(); // Int8 * 4
            }
            if ((flag & (int)Query.OverlayCard) != 0)
            {
                int count = packet.ReadInt32();
                for (int i = 0; i < count; ++i)
                    packet.ReadInt32();
            }
            if ((flag & (int)Query.Counters) != 0)
            {
                int count = packet.ReadInt32();
                for (int i = 0; i < count; ++i)
                    packet.ReadInt32(); // Int16 * 2
            }
            if ((flag & (int)Query.Owner) != 0)
                Owner = duel.GetLocalPlayer(packet.ReadInt32());
            if ((flag & (int)Query.IsDisabled) != 0)
                packet.ReadInt32();
            if ((flag & (int)Query.IsPublic) != 0)
                packet.ReadInt32();
        }
	}
}

