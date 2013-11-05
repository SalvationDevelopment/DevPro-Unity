using UnityEngine;
using System;
using System.Net;
using System.Collections;
using DevPro.Game.Network.Helpers;
using DevPro.Game.Network.Enums;
using DevPro.Game;
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
	

}
