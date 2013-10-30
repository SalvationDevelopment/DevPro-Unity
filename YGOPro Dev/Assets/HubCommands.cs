using UnityEngine;
using System.Collections;
using DevPro.Network;
using DevPro.Network.Enums;
using DevPro.Network.Data;
using JsonFx.Json;
using System;
using System.Text;
using System.Security.Cryptography;

public class HubCommands : MonoBehaviour 
{
	HubClient m_client;
	JsonWriter m_jsonWriter;
	JsonReader m_jsonReader;
	UserData User;

	// Use this for initialization
	void Start () 
	{
		m_client = new HubClient();
		m_jsonWriter = new JsonWriter();
		m_jsonReader = new JsonReader();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_client.Connected())
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
	}
	
	DateTime m_lastRegisterRequest  = new DateTime();
	public void Register(string data) 
	{
		TimeSpan delay = DateTime.Now - m_lastRegisterRequest;
		if(delay.TotalMilliseconds < 5000)
			return;
		LoginRequest loginRequest = m_jsonReader.Read<LoginRequest>(data);
		loginRequest.Password = EncodePassword(loginRequest.Password);
		Connect();
	
		m_client.SendPacket(DevServerPackets.Register,
			m_jsonWriter.Write(loginRequest));
		
		m_lastRegisterRequest = DateTime.Now;
	}

	DateTime m_lastLoginRequest = new DateTime();
	public void Login(string data) 
	{
		TimeSpan delay = DateTime.Now - m_lastLoginRequest;	
		if(User != null || delay.TotalMilliseconds < 5000)
			return;
		LoginRequest loginRequest = m_jsonReader.Read<LoginRequest>(data);
		loginRequest.Password = EncodePassword(loginRequest.Password);
		Connect();
		m_client.SendPacket(DevServerPackets.Login,
			m_jsonWriter.Write(loginRequest));
		m_lastLoginRequest = DateTime.Now;
	}
}
