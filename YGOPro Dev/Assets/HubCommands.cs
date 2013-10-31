using UnityEngine;
using System.Collections;
using DevPro.Network;
using DevPro.Network.Enums;
using DevPro.Network.Data;
using System;
using System.Text;
using System.Security.Cryptography;
using Pathfinding.Serialization.JsonFx;

public class HubCommands : MonoBehaviour 
{
	HubClient m_client;

	// Use this for initialization
	void Start () 
	{
		m_client = new HubClient();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if(m_client != null && m_client.Connected())
		{
			m_client.HandleSendReceive();
		
			while(m_client.HasPacket())
				OnCommand(m_client.GetPacket());
		}
	}
	
	void Connect() 
	{
		if(!m_client.Connected())
			m_client.Connect(ServerDetails.HubAddress,ServerDetails.HubPort);
	}

	string EncodePassword(string password)
	{
		var salt = Encoding.UTF8.GetBytes("&^%£$Ugdsgs:;");
		var userpassword = Encoding.UTF8.GetBytes(password);
	
		var hmacMD5 = new HMACMD5(salt);
		var saltedHash = hmacMD5.ComputeHash(userpassword);
	
		return Convert.ToBase64String(saltedHash);
	}

	void OnCommand(MessageReceived data) 
	{
		Debug.Log(data.Packet);
	//handle incoming packets here
		
		switch(data.Packet)
		{
		case DevClientPackets.LoginAccepted:
			LoginData login = JsonReader.Deserialize<LoginData>(data.GetString());
			ServerDetails.User = new UserData
			{
				rank = login.UserRank,
				username = login.Username,
				team = login.Team,
				teamRank = login.TeamRank
			};
			ServerDetails.LoginKey = login.LoginKey;
			break;
		case DevClientPackets.LoginFailed:
		case DevClientPackets.RegisterFailed:
			//Send Web-Client message
			break;
		case DevClientPackets.GameServers:
			ServerInfo[] servers = JsonReader.Deserialize<ServerInfo[]>(data.GetString());
			ServerDetails.ServerList.Clear();
			foreach(ServerInfo server in servers)
				if(!ServerDetails.ServerList.ContainsKey(server.serverName))
					ServerDetails.ServerList.Add(server.serverName,server);
			break;
		case DevClientPackets.AddServer:
			ServerInfo gameserver = JsonReader.Deserialize<ServerInfo>(data.GetString());
			if(!ServerDetails.ServerList.ContainsKey(gameserver.serverName))
				ServerDetails.ServerList.Add(gameserver.serverName,gameserver);
			break;
		case DevClientPackets.RemoveServer:
			string removeserver = data.GetString();
			if(ServerDetails.ServerList.ContainsKey(removeserver))
				ServerDetails.ServerList.Remove(removeserver);
			break;
			
		}
	}
	
	DateTime m_lastRegisterRequest  = new DateTime();
	public void Register(string data) 
	{
		TimeSpan delay = DateTime.Now - m_lastRegisterRequest;
		if(delay.TotalMilliseconds < 5000)
			return;
		LoginRequest loginRequest = JsonReader.Deserialize<LoginRequest>(data);
		loginRequest.Password = EncodePassword(loginRequest.Password);
		Connect();
	
		m_client.SendPacket(DevServerPackets.Register,
			JsonWriter.Serialize(loginRequest));
		
		m_lastRegisterRequest = DateTime.Now;
	}

	DateTime m_lastLoginRequest = new DateTime();
	public void Login(string data) 
	{
		TimeSpan delay = DateTime.Now - m_lastLoginRequest;	
		if(ServerDetails.User != null || delay.TotalMilliseconds < 5000)
			return;
		LoginRequest loginRequest = JsonReader.Deserialize<LoginRequest>(data);
		loginRequest.Password = EncodePassword(loginRequest.Password);
		Connect();
		m_client.SendPacket(DevServerPackets.Login,
			JsonWriter.Serialize(loginRequest));
		m_lastLoginRequest = DateTime.Now;
	}
}
