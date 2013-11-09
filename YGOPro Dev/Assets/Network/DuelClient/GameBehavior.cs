using System;
using System.Collections.Generic;
using DevPro.Game.Data;
using DevPro.Game.Network.Enums;
using DevPro.Game.Network.Helpers;
using DevPro.Network;
using UnityEngine;

namespace DevPro.Game
{
	public class GameBehavior
	{
		public GameClient Game { get; private set; }
        public GameConnection Connection { get; private set; }

        private IDictionary<StocMessage, Action<GameServerPacket>> m_packets;
        private IDictionary<GameMessage, Action<GameServerPacket>> m_messages;

        public Room m_room { get; private set; }
		public Duel m_duel { get; private set; }
		
		public bool IsTag { get; private set; }
		
		public SelectedCards CardSelection { get; private set; }
		
		public GameBehavior(GameClient game)
        {
            Game = game;
            Connection = game.Connection;

            m_packets = new Dictionary<StocMessage, Action<GameServerPacket>>();
            m_messages = new Dictionary<GameMessage, Action<GameServerPacket>>();
            RegisterPackets();

            m_room = new Room();
			m_duel = new Duel();
        }

        public int GetLocalPlayer(int player)
        {
            return m_duel.IsFirst ? player : 1 - player;
        }
		
		public void OnPacket(GameServerPacket packet)
        {
            StocMessage id = packet.ReadStoc();
            if (id == StocMessage.GameMsg)
            {
                GameMessage msg = packet.ReadGameMsg();
                if (m_messages.ContainsKey(msg))
                    m_messages[msg](packet);
                return;
            }
            if (m_packets.ContainsKey(id))
                m_packets[id](packet);
        }

        private void RegisterPackets()
        {
            m_packets.Add(StocMessage.JoinGame, OnJoinGame);
            m_packets.Add(StocMessage.TypeChange, OnTypeChange);
            m_packets.Add(StocMessage.HsPlayerEnter, OnPlayerEnter);
            m_packets.Add(StocMessage.HsPlayerChange, OnPlayerChange);
            m_packets.Add(StocMessage.SelectHand, OnSelectHand);
            m_packets.Add(StocMessage.SelectTp, OnSelectTp);
            m_packets.Add(StocMessage.TimeLimit, OnTimeLimit);
            m_packets.Add(StocMessage.Replay, OnReplay);
            m_packets.Add(StocMessage.DuelEnd, OnDuelEnd);
			m_packets.Add(StocMessage.Chat, OnPlayerMessage);
			m_packets.Add(StocMessage.ErrorMsg, OnError);

            m_messages.Add(GameMessage.Start, OnStart);
            m_messages.Add(GameMessage.Win, OnWin);
            m_messages.Add(GameMessage.Draw, OnDraw);
            m_messages.Add(GameMessage.ShuffleDeck, OnShuffleDeck);
            m_messages.Add(GameMessage.ShuffleHand, OnShuffleHand);
            m_messages.Add(GameMessage.NewTurn, OnNewTurn);
            m_messages.Add(GameMessage.NewPhase, OnNewPhase);
            m_messages.Add(GameMessage.Damage, OnDamage);
            m_messages.Add(GameMessage.PayLpCost, OnDamage);
            m_messages.Add(GameMessage.Recover, OnRecover);
            m_messages.Add(GameMessage.LpUpdate, OnLpUpdate);
            m_messages.Add(GameMessage.Move, OnMove);
            m_messages.Add(GameMessage.PosChange, OnPosChange);
            m_messages.Add(GameMessage.Chaining, OnChaining);
            m_messages.Add(GameMessage.ChainEnd, OnChainEnd);
            m_messages.Add(GameMessage.SortChain, OnChainSorting);
            m_messages.Add(GameMessage.UpdateCard, OnUpdateCard);
            m_messages.Add(GameMessage.UpdateData, OnUpdateData);

            m_messages.Add(GameMessage.SelectBattleCmd, OnSelectBattleCmd);
            m_messages.Add(GameMessage.SelectCard, OnSelectCard);
            m_messages.Add(GameMessage.SelectChain, OnSelectChain);
            m_messages.Add(GameMessage.SelectCounter, OnSelectCounter);
            m_messages.Add(GameMessage.SelectDisfield, OnSelectDisfield);
            m_messages.Add(GameMessage.SelectEffectYn, OnSelectEffectYn);
            m_messages.Add(GameMessage.SelectIdleCmd, OnSelectIdleCmd);
            m_messages.Add(GameMessage.SelectOption, OnSelectOption);
            m_messages.Add(GameMessage.SelectPlace, OnSelectPlace);
            m_messages.Add(GameMessage.SelectPosition, OnSelectPosition);
            m_messages.Add(GameMessage.SelectSum, OnSelectSum);
            m_messages.Add(GameMessage.SelectTribute, OnSelectTribute);
            m_messages.Add(GameMessage.SelectYesNo, OnSelectYesNo);
            m_messages.Add(GameMessage.AnnounceAttrib, OnAnnounceAttrib);
            m_messages.Add(GameMessage.AnnounceCard, OnAnnounceCard);
            m_messages.Add(GameMessage.AnnounceNumber, OnAnnounceNumber);
            m_messages.Add(GameMessage.AnnounceRace, OnAnnounceRace);
        }

        private void OnJoinGame(GameServerPacket packet)
        {			
			RoomInfo roomInfo = new RoomInfo();
			
			roomInfo.banlist = packet.ReadUInt32();
			roomInfo.rule = packet.ReadByte();
			roomInfo.mode = packet.ReadByte();
			roomInfo.priority = Convert.ToBoolean(packet.ReadByte());
			roomInfo.checkdeck = Convert.ToBoolean(packet.ReadByte());
			roomInfo.shuffledeck = Convert.ToBoolean(packet.ReadByte());
			//c++ padding, skip it
			for(int i = 0; i < 3; i++)
				packet.ReadByte();
			
			roomInfo.startlp = packet.ReadInt32();
			roomInfo.starthand = packet.ReadByte();
			roomInfo.drawcount = packet.ReadByte();
			roomInfo.timer = packet.ReadInt16();
			
			IsTag = roomInfo.mode == 2;
			
			BrowserMessages.RoomInfo(roomInfo);
			
        }

        private void OnTypeChange(GameServerPacket packet)
        {
            int type = packet.ReadByte();
            int pos = type & 0xF;
            m_room.IsHost = ((type >> 4) & 0xF) != 0;
			BrowserMessages.PositionUpdate(pos);
        }

        private void OnPlayerEnter(GameServerPacket packet)
        {
            string name = packet.ReadUnicode(20);
            int pos = packet.ReadByte();
            if (pos < 8)
                m_room.Names[pos] = name;
			BrowserMessages.PlayerEnter(name,pos);
        }

        private void OnPlayerChange(GameServerPacket packet)
        {
            int change = packet.ReadByte();
            int pos = (change >> 4) & 0xF;
            int state = change & 0xF;
            if (pos > 3)
                return;
            if (state < 8)
            {
                string oldname = m_room.Names[pos];
                m_room.Names[pos] = null;
                m_room.Names[state] = oldname;
                m_room.IsReady[pos] = false;
                m_room.IsReady[state] = false;
				BrowserMessages.UpdatePlayer(pos,state);
            }
            else if (state == (int)PlayerChange.Ready)
			{
                m_room.IsReady[pos] = true;
				BrowserMessages.PlayerReady(pos,true);
			}
            else if (state == (int)PlayerChange.NotReady)
			{
                m_room.IsReady[pos] = false;
				BrowserMessages.PlayerReady(pos,false);
			}
            else if (state == (int)PlayerChange.Leave || state == (int)PlayerChange.Observe)
            {
                m_room.IsReady[pos] = false;
                m_room.Names[pos] = null;
				BrowserMessages.PlayerLeave(pos);
            }
        }

        private void OnSelectHand(GameServerPacket packet)
        {
			BrowserMessages.SelectRPS();
        }

        private void OnSelectTp(GameServerPacket packet)
        {
			BrowserMessages.SelectFirstPlayer();
        }

        private void OnTimeLimit(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            if (player == 0)
                Connection.Send(CtosMessage.TimeConfirm);
        }

        private void OnReplay(GameServerPacket packet)
        {
            //byte[] replay = packet.ReadToEnd();

            //const string directory = "replays";
            //if (!Directory.Exists(directory))
                //Directory.CreateDirectory(directory);

            //string otherName = m_room.Names[0] == Program.Config.Username ? m_room.Names[1] : m_room.Names[0];
            //string file = Path.Combine(directory, DateTime.Now.ToString("yyyyMMdd.HHmm") + otherName + ".yrp");

            //try
            //{
                //File.WriteAllBytes(file, replay);
            //}
            //catch (Exception)
            //{
                //continue
            //}
            
            //Connection.Close();
        }        
		
		private void OnWin(GameServerPacket packet)
        {
            int result = packet.ReadByte();
			
			if(result != 2)
				result = GetLocalPlayer(result);
			
			BrowserMessages.OnWin(result);
        }
        
        private void OnDuelEnd(GameServerPacket packet)
        {
			BrowserMessages.OnDuelEnd();
            Connection.Close();
        }

        private void OnStart(GameServerPacket packet)
        {
			DuelStart data = new DuelStart();
			
			int type = packet.ReadByte();
			m_duel.IsFirst = (type & 0xF) == 0;
			data.IsFirst = m_duel.IsFirst;
			
            m_duel.LifePoints[GetLocalPlayer(0)] = packet.ReadInt32();
            m_duel.LifePoints[GetLocalPlayer(1)] = packet.ReadInt32();
			data.LifePoints = m_duel.LifePoints;
            data.PlayerOneDeckSize = packet.ReadInt16();
            data.PlayerOneExtraSize = packet.ReadInt16();
			m_duel.Fields[GetLocalPlayer(0)].Init(data.PlayerOneDeckSize, data.PlayerOneExtraSize);
            data.PlayerTwoDeckSize = packet.ReadInt16();
            data.PlayerTwoExtraSize = packet.ReadInt16();
			m_duel.Fields[GetLocalPlayer(1)].Init(data.PlayerTwoDeckSize, data.PlayerTwoExtraSize);
			
			BrowserMessages.SendStartDuel(data);
        }



        private void OnDraw(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int count = packet.ReadByte();
			
			for (int i = 0; i < count; ++i)
            {
                m_duel.Fields[player].Deck.RemoveAt(m_duel.Fields[player].Deck.Count - 1);
                m_duel.Fields[player].Hand.Add(new CardData() {Id = 0, Location = (int)CardLocation.Hand, Position = 0 });
            }
			
			BrowserMessages.DrawCard(player,count);
        }

        private void OnShuffleDeck(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
			BrowserMessages.ShuffleDeck(player);
        }

        private void OnShuffleHand(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            packet.ReadByte(); // count not required
            foreach (CardData card in m_duel.Fields[player].Hand)
                card.Id = packet.ReadInt32();
			
			BrowserMessages.ShuffleHand(player,m_duel.Fields[player].Hand);
        }

        private void OnNewTurn(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
			BrowserMessages.NewTurn(player);
        }

        private void OnNewPhase(GameServerPacket packet)
        {
            Phase phase = (Phase)packet.ReadByte();
			BrowserMessages.NewPhase((int)phase);
        }

        private void OnDamage(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int total = packet.ReadInt32();
			BrowserMessages.PlayerDamage(player,total);
        }

        private void OnRecover(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int total = packet.ReadInt32();
			
			BrowserMessages.PlayerRecover(player,total);
        }

        private void OnLpUpdate(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int count = packet.ReadInt32();
			
			BrowserMessages.UpdateLifePoints(player,count);
        }

        private void OnMove(GameServerPacket packet)
        {
            int cardId = packet.ReadInt32();
            int pc = GetLocalPlayer(packet.ReadByte());
            int pl = packet.ReadByte();
            int ps = packet.ReadSByte();
            packet.ReadSByte(); // pp
            int cc = GetLocalPlayer(packet.ReadByte());
            int cl = packet.ReadByte();
            int cs = packet.ReadSByte();
            int cp = packet.ReadSByte();
            packet.ReadInt32(); // reason

            CardData card = m_duel.GetCard(pc, (CardLocation)pl, ps);

            m_duel.RemoveCard((CardLocation)pl, card, pc, ps);
            m_duel.AddCard((CardLocation)cl, cardId, cc, cs, cp);
			
			BrowserMessages.MoveCard(pc,(int)pl,ps,cc,(int)cl,cs,cp);
        }

        private void OnPosChange(GameServerPacket packet)
        {
            packet.ReadInt32(); // card id
            int pc = GetLocalPlayer(packet.ReadByte());
            int pl = packet.ReadByte();
            int ps = packet.ReadSByte();
            packet.ReadSByte(); // pp
            int cp = packet.ReadSByte();
            CardData card = m_duel.GetCard(pc, (CardLocation)pl, ps);
            if (card != null)
                card.Position = cp;
			
			BrowserMessages.ChangeCardPosition(pc,(int)pl,ps,cp);
        }

        private void OnChaining(GameServerPacket packet)
        {
//            packet.ReadInt32(); // card id
//            int pcc = GetLocalPlayer(packet.ReadByte());
//            int pcl = packet.ReadByte();
//            int pcs = packet.ReadSByte();
//            packet.ReadSByte(); // subs
//            ClientCard card = m_duel.GetCard(pcc, (CardLocation)pcl, pcs);
//            int cc = GetLocalPlayer(packet.ReadByte());
//            m_ai.OnChaining(card, cc);
        }

        private void OnChainEnd(GameServerPacket packet)
        {
            //m_ai.OnChainEnd();
        }

        private void OnChainSorting(GameServerPacket packet)
        {
            //AutoChain??
            Connection.Send(CtosMessage.Response,-1);
        }

        private void OnUpdateCard(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int loc = packet.ReadByte();
            int index = packet.ReadByte();

            packet.ReadInt32(); // ???
			
			CardData card = new CardData();
			
			card.Update(packet,m_duel);
			
			BrowserMessages.UpdateCard(player,loc,index,card);
        }

        private void OnUpdateData(GameServerPacket packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            CardLocation loc = (CardLocation)packet.ReadByte();

            IList<CardData> cards = null;
            switch (loc)
            {
                case CardLocation.Hand:
                    cards = m_duel.Fields[player].Hand;
                    break;
                case CardLocation.MonsterZone:
                    cards = m_duel.Fields[player].MonsterZone;
                    break;
                case CardLocation.SpellZone:
                    cards = m_duel.Fields[player].SpellZone;
                    break;
                case CardLocation.Grave:
                    cards = m_duel.Fields[player].Graveyard;
                    break;
                case CardLocation.Removed:
                    cards = m_duel.Fields[player].Banished;
                    break;
                case CardLocation.Deck:
                    cards = m_duel.Fields[player].Deck;
                    break;
                case CardLocation.Extra:
                    cards = m_duel.Fields[player].ExtraDeck;
                    break;
            }
            if (cards != null)
            {
                foreach (CardData card in cards)
                {
                    int len = packet.ReadInt32();
                    if (len < 8) continue;
                    long pos = packet.GetPosition();
                    card.Update(packet,m_duel);
                    packet.SetPosition(pos + len - 4);
                }
            }
			
			BrowserMessages.UpdateCards(player,(int)loc,cards);
        }

        private void OnSelectBattleCmd(GameServerPacket packet)
        {
            packet.ReadByte(); // player
            m_duel.BattlePhase = new BattlePhase();
			m_duel.BattlePhase.Init();
            BattlePhase battle = m_duel.BattlePhase;

            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();

                CardData card = m_duel.GetCard(con, loc, seq);
                if (card != null)
                {
                    card.ActionIndex[0] = i;
                    battle.ActivableCards.Add(card);
                    battle.ActivableDescs.Add(desc);
                }
            }

            count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                packet.ReadByte(); // diratt

                CardData card = m_duel.GetCard(con, loc, seq);
                if (card != null)
                {
                    card.ActionIndex[1] = i;
                    battle.AttackableCards.Add(m_duel.GetCard(con, loc, seq));
                }
            }

            battle.CanMainPhaseTwo = packet.ReadByte() != 0;
            battle.CanEndPhase = packet.ReadByte() != 0;
			
			BrowserMessages.BattleCommands(battle);
        }

        private void InternalOnSelectCard(GameServerPacket packet)
        {
			CardSelection = new SelectedCards();
			
            packet.ReadByte(); // player
            CardSelection.Cancelable = packet.ReadByte() != 0;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                packet.ReadByte(); // pos
                CardPos card = new CardPos() { Player = player, Loc = (int)loc, Index = seq };
                CardSelection.Cards.Add(card);
            }
			
			BrowserMessages.SelectCards(CardSelection.Cards,min,max,Convert.ToInt32(CardSelection.Cancelable));
        }

        private void OnSelectCard(GameServerPacket packet)
        {
            InternalOnSelectCard(packet);
        }

        private void OnSelectChain(GameServerPacket packet)
        {
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            packet.ReadByte(); // specount
            bool forced = packet.ReadByte() != 0;
            packet.ReadInt32(); // hint1
            packet.ReadInt32(); // hint2

            IList<CardPos> cards = new List<CardPos>();
            IList<int> descs = new List<int>();

            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();
                cards.Add(new CardPos() { Player = con, Loc = (int)loc, Index = seq });
                descs.Add(desc);
            }

            if (cards.Count == 0)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            if (cards.Count == 1 && forced)
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }
			BrowserMessages.OnChaining(cards,descs,forced);
        }

        private void OnSelectCounter(GameServerPacket packet)
        {
//            packet.ReadByte(); // player
//            int type = packet.ReadInt16();
//            int quantity = packet.ReadByte();
//
//            IList<ClientCard> cards = new List<ClientCard>();
//            IList<int> counters = new List<int>();
//            int count = packet.ReadByte();
//            for (int i = 0; i < count; ++i)
//            {
//                packet.ReadInt32(); // card id
//                int player = GetLocalPlayer(packet.ReadByte());
//                CardLocation loc = (CardLocation) packet.ReadByte();
//                int seq = packet.ReadByte();
//                int num = packet.ReadByte();
//                cards.Add(m_duel.GetCard(player, loc, seq));
//                counters.Add(num);
//            }

//            IList<int> used = m_ai.OnSelectCounter(type, quantity, cards, counters);
//            byte[] result = new byte[used.Count];
//            for (int i = 0; i < quantity; ++i)
//                result[i] = (byte) used[i];
//            GameClientPacket reply = new GameClientPacket(CtosMessage.Response);
//            reply.Write(result);
//            Connection.Send(reply);
        }

        private void OnSelectDisfield(GameServerPacket packet)
        {
            OnSelectPlace(packet);
        }

        private void OnSelectEffectYn(GameServerPacket packet)
        {
            packet.ReadByte(); // player

            int cardId = packet.ReadInt32();
            int player = GetLocalPlayer(packet.ReadByte());
            CardLocation loc = (CardLocation)packet.ReadByte();
            int seq = packet.ReadByte();
            packet.ReadByte();

            CardData card = m_duel.GetCard(player, loc, seq);
            if (card == null)
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }

            if (card.Id == 0) 
				card.Id = cardId;
			
			BrowserMessages.ActivateCardEffect(cardId);
        }

        private void OnSelectIdleCmd(GameServerPacket packet)
        {
            packet.ReadByte(); // player

            m_duel.MainPhase = new MainPhase();
			m_duel.MainPhase.Init();
            MainPhase main = m_duel.MainPhase;
            int count;
            for (int k = 0; k < 5; k++)
            {
                count = packet.ReadByte();
                for (int i = 0; i < count; ++i)
                {
                    packet.ReadInt32(); // card id
                    int con = GetLocalPlayer(packet.ReadByte());
                    CardLocation loc = (CardLocation)packet.ReadByte();
                    int seq = packet.ReadByte();
                    CardData card = m_duel.GetCard(con, loc, seq);
                    if (card == null) continue;
                    card.ActionIndex[k] = i;
                    switch (k)
                    {
                        case 0:
                            main.SummonableCards.Add(card);
                            break;
                        case 1:
                            main.SpecialSummonableCards.Add(card);
                            break;
                        case 2:
                            main.ReposableCards.Add(card);
                            break;
                        case 3:
                            main.MonsterSetableCards.Add(card);
                            break;
                        case 4:
                            main.SpellSetableCards.Add(card);
                            break;
                    }
                }
            }
            count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();

                CardData card = m_duel.GetCard(con, loc, seq);
                if (card == null) continue;
                card.ActionIndex[5] = i;
                if (card.ActionActivateIndex.ContainsKey(desc))
                    card.ActionActivateIndex.Remove(desc);
                card.ActionActivateIndex.Add(desc, i);
                main.ActivableCards.Add(card);
                main.ActivableDescs.Add(desc);
            }

            main.CanBattlePhase = packet.ReadByte() != 0;
            main.CanEndPhase = packet.ReadByte() != 0;

            BrowserMessages.IdleCommands(main);
        }

        private void OnSelectOption(GameServerPacket packet)
        {
            IList<int> options = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                options.Add(packet.ReadInt32());
			BrowserMessages.SelectOption(options);
        }

        private void OnSelectPlace(GameServerPacket packet)
        {
            packet.ReadByte(); // player
            packet.ReadByte(); // min
            int field = ~packet.ReadInt32();

            byte[] resp = new byte[3];
            resp[0] = (byte)GetLocalPlayer(0);

            int filter;
            if ((field & 0x1f) != 0)
            {
                resp[1] = 0x4;
                filter = field & 0x1f;
            }
            else if ((field & 0x1f00) != 0)
            {
                resp[1] = 0x8;
                filter = (field >> 8) & 0x1f;
            }
            else if ((field & 0x1f0000) != 0)
            {
                resp[1] = 0x4;
                filter = (field >> 16) & 0x1f;
            }
            else
            {
                resp[1] = 0x8;
                filter = (field >> 24) & 0x1f;
            }

            if ((filter & 0x4) != 0) resp[2] = 2;
            else if ((filter & 0x2) != 0) resp[2] = 1;
            else if ((filter & 0x8) != 0) resp[2] = 3;
            else if ((filter & 0x1) != 0) resp[2] = 0;
            else if ((filter & 0x10) != 0) resp[2] = 4;

            GameClientPacket reply = new GameClientPacket(CtosMessage.Response);
            reply.Write(resp);
            Connection.Send(reply);
        }

        private void OnSelectPosition(GameServerPacket packet)
        {
            packet.ReadByte(); // player
            int cardId = packet.ReadInt32();
            int pos = packet.ReadByte();
            if (pos == 0x1 || pos == 0x2 || pos == 0x4 || pos == 0x8)
            {
                Connection.Send(CtosMessage.Response, pos);
                return;
            }
            IList<int> positions = new List<int>();
            if ((pos & (int)CardPosition.FaceUpAttack) != 0)
                positions.Add((int)CardPosition.FaceUpAttack);
            if ((pos & (int)CardPosition.FaceDownAttack) != 0)
                positions.Add((int)CardPosition.FaceDownAttack);
            if ((pos & (int)CardPosition.FaceUpDefence) != 0)
                positions.Add((int)CardPosition.FaceUpDefence);
            if ((pos & (int)CardPosition.FaceDownDefence) != 0)
                positions.Add((int)CardPosition.FaceDownDefence);
			
			BrowserMessages.SelectPosition(positions);
        }

        private void OnSelectSum(GameServerPacket packet)
        {
			CardSelection = new SelectedCards();
			
            packet.ReadByte(); // mode
            packet.ReadByte(); // player
            int sumval = packet.ReadInt32();
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            CardSelection.Cards = new List<CardPos>();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int cardId = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                CardPos card = new CardPos() { Player = player, Loc = (int)loc, Index = seq };
                CardSelection.Cards.Add(card);
                packet.ReadInt32();
            }
			
			BrowserMessages.SelectSyncoMaterial(sumval,min,max,CardSelection.Cards);
        }

        private void OnSelectTribute(GameServerPacket packet)
        {
            InternalOnSelectCard(packet);
        }

        private void OnSelectYesNo(GameServerPacket packet)
        {
            packet.ReadByte(); // player
			BrowserMessages.SelectYn(packet.ReadInt32());
        }

        private void OnAnnounceAttrib(GameServerPacket packet)
        {
            IList<int> attributes = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            int available = packet.ReadInt32();
            int filter = 0x1;
            for (int i = 0; i < 7; ++i)
            {
                if ((available & filter) != 0)
                    attributes.Add((int)filter);
                filter <<= 1;
            }
			BrowserMessages.AnnounceAttribute(count,attributes);
        }

        private void OnAnnounceCard(GameServerPacket packet)
        {
            BrowserMessages.AnnounceCard();
        }

        private void OnAnnounceNumber(GameServerPacket packet)
        {
            IList<int> numbers = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                numbers.Add(packet.ReadInt32());
            BrowserMessages.AnnounceNumber(numbers);
        }

        private void OnAnnounceRace(GameServerPacket packet)
        {
            IList<int> races = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            int available = packet.ReadInt32();
            int filter = 0x1;
            for (int i = 0; i < 23; ++i)
            {
                if ((available & filter) != 0)
                    races.Add((int)filter);
                filter <<= 1;
            }
			BrowserMessages.AnnounceRace(count, races);
        }
		
		private void OnPlayerMessage(GameServerPacket packet)
		{
			int player = packet.ReadInt16();
			string message = packet.ReadUnicode();
            BrowserMessages.PlayerMessage(player,message);
		}
		
		private void OnError(GameServerPacket packet)
		{
			int error = packet.ReadByte();
			
			switch(error)
			{
			case 2: //Deck Error
				// C++ padding, skip it
                    for (int i = 0; i < 3; i++)
                        packet.ReadByte();
                int cardid = packet.ReadInt32();
				BrowserMessages.DeckError(cardid);
				break;
			case 3: //Side Error
				BrowserMessages.SideError();
				break;
			}
		}
	}
}

