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
	
	public const short Version = 0x1321;
	public GameConnection Connection { get; private set; }
	private GameBehavior m_behavior;
	private Deck Deck;
	
	public bool LobbyButtons = true;
	public bool RPS;
	public bool FirstSelect;
	
	void Start()
	{
		m_behavior = new GameBehavior(this);	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Connection != null)
		{
			while (Connection.HasPacket())
			{
				GameServerPacket packet = Connection.Receive();
				m_behavior.OnPacket(packet);
			}
		}
	}
	
	void OnGUI()
	{
		if(LobbyButtons)
		{
			var hubclient = GameObject.Find("HubClient");
			if(GUI.Button(new Rect(Screen.width /2 + 50, 15,100,30),"Login"))
			{
				if(ServerDetails.User == null)
				{
					hubclient.SendMessage("Login",
					JsonWriter.Serialize(new LoginRequest() { Username = "Unity", Password = "nuts", UID = "Unity" }));
				}
			}
		
			if(GUI.Button(new Rect(Screen.width /2 + 155, 15,100,30),"Create Game"))
			{
				if(ServerDetails.User == null)
					return;
			
					CreateGame("200OOO8000,0,5,1,U,V34OG");
					Debug.Log("Game Created");
			}
		
			if(GUI.Button(new Rect(Screen.width /2 + 260, 15,100,30),
			ServerDetails.User == null ? "Not Ready": m_behavior.m_room.Ready(ServerDetails.User.username) ? "Ready": "Not Ready"))
			{
				if(ServerDetails.User == null) return;
			
				UpdateDeck(JsonWriter.Serialize(
				new Deck(){ 
					main = new List<int>()
					{ 
						98777036,98777036,61468779,62950604,62950604,62950604,98358303,
						98358303,98358303,21454943,21454943,21454943,85087012,25343017,
						25343017,99070951,99070951,99070951,52430902,52430902,23434538,
						36484016,36484016,36484016,37520316,53129443,5318639,5318639,
						67723438,67723438,67723438,44095762,44095762,53582587,82633308,
						59718521,59718521,84749824,5288597,5288597
					},
					extra = new List<int>()
					{
						40101111,99916754,95526884,80321197,70780151,45815891,45379225,
						73580471,2956282,43385557,33198837,26593852,7582066,15028680,95992081
					},
					side = new List<int>()
				
				}));
			
				SetReady(m_behavior.m_room.Ready(ServerDetails.User.username) ? 0: 1);
			}
		
			if(GUI.Button(new Rect(Screen.width /2 + 365, 15,100,30),"Start Duel"))
			{
				if(m_behavior.m_room.IsHost)
					StartDuel();
			}
		}
		if(RPS)
		{
			if(GUI.Button(new Rect(Screen.width /2 + (Screen.width /4) - 150, Screen.height /2,100,30),"Rock"))
			{
				SelectRPS(1);
			}
			if(GUI.Button(new Rect(Screen.width /2 + (Screen.width /4) - 45, Screen.height /2,100,30),"Paper"))
			{
				SelectRPS(2);
			}
			if(GUI.Button(new Rect(Screen.width /2 + (Screen.width /4) + 60, Screen.height /2,100,30),"Scissors"))
			{
				SelectRPS(3);
			}
		}
		
		if(FirstSelect)
		{
			if(GUI.Button(new Rect(Screen.width /2 + (Screen.width /4) - 95, Screen.height /2,100,30),"First"))
			{
				SelectFirstPlayer(1);
			}
			if(GUI.Button(new Rect(Screen.width /2 + (Screen.width /4) + 10, Screen.height /2,100,30),"Second"))
			{
				SelectFirstPlayer(0);
			}
		}
		
		GUI.Box(new Rect(Screen.width /2 + 25,50,Screen.width /2 - 30,Screen.height /2 - 55),"Player 2 Info");
		
		GUI.Box(new Rect(Screen.width /2 + 25,Screen.height /2 + 35,Screen.width /2 - 30,Screen.height /2 - 55),"Player 1 Info");
		
		//player 2 info
		GUI.Label(new Rect(Screen.width /2 + 30, 195,300,30), "Card Info");
		
		GUI.Button(new Rect(Screen.width /2 + 25,Screen.height /2 - 40,85,30),
			"Deck: " + m_behavior.m_duel.Fields[1].Deck.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(1 * 90), Screen.height /2 - 40,85,30),
			"Grave: " + m_behavior.m_duel.Fields[1].Graveyard.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(2 * 90),Screen.height /2 - 40,85,30),
			"Removed: " + m_behavior.m_duel.Fields[1].Banished.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(3 * 90),Screen.height /2 - 40,85,30),
			"Extra: " + m_behavior.m_duel.Fields[1].ExtraDeck.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(4 * 90),Screen.height /2 - 40,85,30),
			"Hand: " + m_behavior.m_duel.Fields[1].Hand.Count);
		
		GUI.Label(new Rect(Screen.width /2 + 30, 75,300,30), "Monster Zone");
		
		for(int i = 0; i < m_behavior.m_duel.Fields[1].MonsterZone.Length; i++)
		{
			Rect loc = new Rect(Screen.width /2 + 25 +(i * 90), 100,85,30);
			string name;
			
			if(m_behavior.m_duel.Fields[1].MonsterZone[i] == null)
				name = "Empty";
			else
			{
				if(m_behavior.m_duel.Fields[1].MonsterZone[i].Id == 0)
					name = "Unknown";
				else
					name = m_behavior.m_duel.Fields[1].MonsterZone[i].Id.ToString();
			}
			GUI.Button(loc,name);
		}
		
		GUI.Label(new Rect(Screen.width /2 + 30, 140,300,30), "Spell/Trap Zone");
		
		for(int i = 0; i < m_behavior.m_duel.Fields[1].SpellZone.Length - 1; i++)
		{
			Rect loc = new Rect(Screen.width /2 + 25 +(i * 90), 165,85,30);
			string name;
			
			if(m_behavior.m_duel.Fields[1].SpellZone[i] == null)
				name = "Empty";
			else
			{
				if(m_behavior.m_duel.Fields[1].SpellZone[i].Id == 0)
					name = "Unknown";
				else
					name = m_behavior.m_duel.Fields[1].SpellZone[i].Id.ToString();
			}
			GUI.Button(loc,name);
		}
		
		//player 1 info
		
		GUI.Label(new Rect(Screen.width /2 + 30, Screen.height /2 - 15 +195,300,30), "Card Info");
		
		GUI.Button(new Rect(Screen.width /2 + 25,Screen.height /2 - 15 + Screen.height /2 - 40,85,30),
			"Deck: " + m_behavior.m_duel.Fields[0].Deck.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(1 * 90), Screen.height /2 - 15 +Screen.height /2 - 40,85,30),
			"Grave: " + m_behavior.m_duel.Fields[0].Graveyard.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(2 * 90),Screen.height /2 - 15 +Screen.height /2 - 40,85,30),
			"Removed: " + m_behavior.m_duel.Fields[0].Banished.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(3 * 90),Screen.height /2 - 15 +Screen.height /2 - 40,85,30),
			"Extra: " + m_behavior.m_duel.Fields[0].ExtraDeck.Count);
		GUI.Button(new Rect(Screen.width /2 + 25 +(4 * 90),Screen.height /2 - 15 +Screen.height /2 - 40,85,30),
			"Hand: " + m_behavior.m_duel.Fields[0].Hand.Count);
		
		GUI.Label(new Rect(Screen.width /2 + 30, Screen.height /2 - 15 +75,300,30), "Monster Zone");
		
		for(int i = 0; i < m_behavior.m_duel.Fields[0].MonsterZone.Length; i++)
		{
			Rect loc = new Rect(Screen.width /2 + 25 +(i * 90),Screen.height /2 - 15 + 100,85,30);
			string name;
			
			if(m_behavior.m_duel.Fields[0].MonsterZone[i] == null)
				name = "Empty";
			else
			{
				if(m_behavior.m_duel.Fields[0].MonsterZone[i].Id == 0)
					name = "Unknown";
				else
					name = m_behavior.m_duel.Fields[0].MonsterZone[i].Id.ToString();
			}
			GUI.Button(loc,name);
		}
		
		GUI.Label(new Rect(Screen.width /2 + 30,Screen.height /2 - 15 + 140,300,30), "Spell/Trap Zone");
		
		for(int i = 0; i < m_behavior.m_duel.Fields[0].SpellZone.Length - 1; i++)
		{
			Rect loc = new Rect(Screen.width /2 + 25 +(i * 90),Screen.height /2 - 15 + 165,85,30);
			string name;
			
			if(m_behavior.m_duel.Fields[0].SpellZone[i] == null)
				name = "Empty";
			else
			{
				if(m_behavior.m_duel.Fields[0].SpellZone[i].Id == 0)
					name = "Unknown";
				else
					name = m_behavior.m_duel.Fields[0].SpellZone[i].Id.ToString();
			}
			GUI.Button(loc,name);
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
		{
			Connection.Send(CtosMessage.HsStart);
			LobbyButtons = false;
		}
	}
	//value must be between 1-3
	public void SelectRPS(int selected)
	{
		if(selected > 0 && selected < 4)
		{
			RPS = false;
			Connection.Send(CtosMessage.HandResult, (byte)selected);
		}
	}
	//1 means to go first
	public void SelectFirstPlayer(int first)
	{
		if(first == 0 || first == 1)
		{
			FirstSelect = false;
			Connection.Send(CtosMessage.TpResult,(byte)first);
		}
	}
	//return an empty array to cancel selection
	public void SelectCardsReply(string data)
	{
		if(m_behavior.CardSelection == null)
			return;
		
		IList<CardPos> selectedInfo = JsonReader.Deserialize<List<CardPos>>(data);
		IList<CardData> selected = new List<CardData>();
		
		foreach(CardPos info in selectedInfo)
			selected.Add(m_behavior.m_duel.GetCard(info.Player,(CardLocation)info.Loc,info.Index));

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
		IList<CardPos> selectedInfo = JsonReader.Deserialize<List<CardPos>>(data);
		IList<CardData> selected = new List<CardData>();
		
		foreach(CardPos info in selectedInfo)
			selected.Add(m_behavior.m_duel.GetCard(info.Player,(CardLocation)info.Loc,info.Index));

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
