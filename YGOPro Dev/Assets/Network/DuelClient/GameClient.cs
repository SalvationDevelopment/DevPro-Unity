using UnityEngine;
using System;
using System.Net;
using System.Collections.Generic;
using DevPro.Game.Network.Helpers;
using DevPro.Game.Network.Enums;
using DevPro.Game;
using DevPro.Game.Data;
using DevPro.Network;
using DevPro.Network.Data;
using Pathfinding.Serialization.JsonFx;

public class GameClient : MonoBehaviour {
	
	public const short Version = 0x1320;
	public GameConnection Connection { get; private set; }
	private GameBehavior m_behavior;
	private Deck Deck;
	
	// Update is called once per frame
	void Update () 
	{
		if(Connection != null)
		{
			while (Connection.HasPacket())
			{
				GameServerPacket packet = Connection.Receive();
				Debug.Log ("GamePacket: " +(StocMessage)packet.Content[0]);
				m_behavior.OnPacket(packet);
			}
		}
	}
	
	public void CreateGame(string roomInfos)
	{
		if(Connection != null)
			if(Connection.IsConnected)
				Connection.Close();
		
		ServerInfo server = ServerDetails.GetRandomServer();
		if(server != null && ServerDetails.User != null)
		{
			Connection = new GameConnection(IPAddress.Parse(server.serverAddress),server.serverPort);
			m_behavior = new GameBehavior(this);
	    	GameClientPacket packet = new GameClientPacket(CtosMessage.PlayerInfo);
        	packet.Write(ServerDetails.User.username + "$" + ServerDetails.LoginKey, 20);
        	Connection.Send(packet);

        	byte[] junk = {0xCC, 0xCC, 0x00, 0x00, 0x00, 0x00};
        	packet = new GameClientPacket(CtosMessage.JoinGame);
        	packet.Write(Version);
        	packet.Write(junk);
        	packet.Write(roomInfos, 30);
        	Connection.Send(packet);
		}
		else
		{
			//send no servers avliable message
			BrowserMessages.MessagePopUp("No servers are currently available.");
		}
	
	}
	
	public void UpdateDeck(string data)
	{
		Deck = JsonReader.Deserialize<Deck>(data);
	}
	
	public void SetReady(int ready)
	{
		if(Deck == null)
		{
			BrowserMessages.MessagePopUp("Deck information required.");
			return;	
		}
		
		if(Convert.ToBoolean(ready))
		{
			//send deck information here
			GameClientPacket deck = new GameClientPacket(CtosMessage.UpdateDeck);
            deck.Write(Deck.main.Count + Deck.extra.Count);
            deck.Write(Deck.side.Count);
            foreach (int card in Deck.main)
                deck.Write(card);
            foreach (int card in Deck.extra)
                deck.Write(card);
            foreach (int card in Deck.side)
                deck.Write(card);
            Connection.Send(deck);
		}
		
		Connection.Send(Convert.ToBoolean(ready) ? CtosMessage.HsReady: CtosMessage.HsNotReady);
	}
	
	public void StartDuel()
	{            
		if (m_behavior.m_room.IsHost && 
			(!m_behavior.IsTag ? m_behavior.m_room.IsReady[0] && m_behavior.m_room.IsReady[1]:
			m_behavior.m_room.IsReady[0] && m_behavior.m_room.IsReady[1] 
				&& m_behavior.m_room.IsReady[2] && m_behavior.m_room.IsReady[3]))
			Connection.Send(CtosMessage.HsStart);	
	}
	//value must be between 1-3
	public void SelectRPS(int selected)
	{
		if(selected > 0 && selected < 4)
			Connection.Send(CtosMessage.HandResult, (byte)selected);
	}
	//1 means to go first
	public void SelectFirstPlayer(int first)
	{
		if(first == 0 || first == 1)
			Connection.Send(CtosMessage.TpResult,(byte)first);
	}
	//return an empty array to cancel selection
	public void SelectCardsReply(string data)
	{
		if(m_behavior.CardSelection == null)
			return;
		    IList<CardData> selected = JsonReader.Deserialize<IList<CardData>>(data);

            if (selected.Count == 0 && m_behavior.CardSelection.Cancelable)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            byte[] result = new byte[selected.Count + 1];
            result[0] = (byte)selected.Count;
            for (int i = 0; i < selected.Count; ++i)
            {
                int id = 0;
                for (int j = 0; j < m_behavior.CardSelection.Cards.Count; ++j)
                {
                    if (m_behavior.CardSelection.Cards[j] == null) continue;
                    if (m_behavior.CardSelection.Cards[j].Equals(selected[i]))
                    {
                        id = j;
                        break;
                    }
                }
                result[i + 1] = (byte)id;
            }

            GameClientPacket reply = new GameClientPacket(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
	}
	
	public void SendYn(int result)
	{
		bool accept = Convert.ToBoolean(result);
		Connection.Send(CtosMessage.Response, accept ? 1:0);
	}
	
	public void SendPosition(int pos)
	{
		Connection.Send(CtosMessage.Response, pos);	
	}
	
	public void SendAnnounceCard(int cardid)
	{
		Connection.Send(CtosMessage.Response, cardid);
	}
	
	public void SendAnnounceAttributes(string data)
	{
		IList<int> attributes = JsonReader.Deserialize<List<int>>(data);
		int reply = 0;
		for (int i = 0; i < attributes.Count; ++i)
			reply += (int)attributes[i];
		Connection.Send(CtosMessage.Response, reply);	
	}
	
	public void SendAnnounceNumber(int number)
	{
		Connection.Send(CtosMessage.Response, number);	
	}
	
	public void SendAnnounceRace(string data)
	{
		IList<int> races = JsonReader.Deserialize<List<int>>(data);
		int reply = 0;
		for (int i = 0; i < races.Count; ++i)
			reply += races[i];
		Connection.Send(CtosMessage.Response, reply);	
	}
	
	public void SendOption(int option)
	{
		Connection.Send(CtosMessage.Response, option);	
	}
	
	public void MainPhaseAction(string data)
	{
		MainPhaseAction action = JsonReader.Deserialize<MainPhaseAction>(data);
		Connection.Send(CtosMessage.Response, action.ToValue());
	}
	
	public void BattlePhaseAction(string data)
	{
		BattlePhaseAction action = JsonReader.Deserialize<BattlePhaseAction>(data);
		Connection.Send(CtosMessage.Response, action.ToValue());
	}
	
	public void SyncroMaterialSelection(string data)
	{
		IList<CardData> selected = JsonReader.Deserialize<List<CardData>>(data);

		byte[] result = new byte[selected.Count + 1];
		result[0] = (byte)selected.Count;
		for (int i = 0; i < selected.Count; ++i)
		{
			int id = 0;
			for (int j = 0; j < m_behavior.CardSelection.Cards.Count; ++j)
			{
				if (m_behavior.CardSelection.Cards[j] == null) continue;
				if (m_behavior.CardSelection.Cards[j].Equals(selected[i]))
				{
					id = j;
					break;
				}
			}
			result[i + 1] = (byte)id;
		}

		GameClientPacket reply = new GameClientPacket(CtosMessage.Response);
		reply.Write(result);
		Connection.Send(reply);	
	}
	
	public void SendOnChain(int chain)
	{
		Connection.Send(CtosMessage.Response, chain);	
	}
	

}
