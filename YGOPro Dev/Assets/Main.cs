using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using DevPro.Network;
using DevPro.Network.Enums;
using DevPro.Network.Data;
using JsonFx.Json;

public class Main : MonoBehaviour
{
	public static Main Game;
	public static UserData User;
	private HubClient HubClient = new HubClient();
	//private JsonReader m_jsonReader;
	private JsonWriter m_jsonWriter;
	
	void Start()
	{
		//m_jsonReader = new JsonReader();
		m_jsonWriter = new JsonWriter();
		Game = this; // Global access
		new GameObject("LoginScreen",typeof(LoginScreen));//load loginscreen
	}
	
	//updates every frame
	void Update()
	{
		if(HubClient.Connected())
		{
			HubClient.HandleSendReceive();
			
			while(HubClient.HasPacket())
				OnCommand(HubClient.GetPacket());
		}
	}
	
	void OnCommand(MessageReceived data)
	{
		Debug.Log(data.Packet);
		switch(data.Packet)
		{
		case DevClientPackets.LoginAccepted:
			break;
		case DevClientPackets.LoginFailed:
			break;
		case DevClientPackets.RegisterAccept:
			break;
		case DevClientPackets.RegisterFailed:
			break;
		default:
			break;
		}
	}
	
	void Connect()
	{
		if(!HubClient.Connected())
			HubClient.Connect(ServerDetails.HubAddress,ServerDetails.HubPort);
	}
	
	DateTime m_lastLoginRequest = new DateTime();
	public void Login(string username, string password)
	{
		TimeSpan delay = DateTime.Now - m_lastLoginRequest;
		if(User != null || delay.TotalMilliseconds < 5000)
			return;
		Connect();
		HubClient.SendPacket(DevServerPackets.Login,
			m_jsonWriter.Write(new LoginRequest() 
		{ 
			Username = username, 
			Password =  EncodePassword(password), 
			UID = "Test"
		}));
		m_lastLoginRequest = DateTime.Now;
	}
	
	DateTime m_lastRegisterRequest = new DateTime();
	public void Register(string username,string password)
	{
		TimeSpan delay = DateTime.Now - m_lastRegisterRequest;
		if(delay.TotalMilliseconds < 5000)
			return;
		Connect();
		HubClient.SendPacket(DevServerPackets.Register, 
			m_jsonWriter.Write(new LoginRequest
		{ 
			Username = username, 
			Password= EncodePassword(password), 
			UID= "Unity"
		}));
		
		m_lastRegisterRequest = DateTime.Now;
	}
	
	private string EncodePassword(string password)
	{
		var salt = Encoding.UTF8.GetBytes("&^%£$Ugdsgs:;");
		var userpassword = Encoding.UTF8.GetBytes(password);

		var hmacMD5 = new HMACMD5(salt);
		var saltedHash = hmacMD5.ComputeHash(userpassword);

		//Convert encoded bytes back to a 'readable' string
		return Convert.ToBase64String(saltedHash);
	}
	
	public void SendMessage(MessageType type, CommandType command, string channel, string message)
	{
		HubClient.SendPacket(DevServerPackets.ChatMessage, 
			m_jsonWriter.Write(new ChatMessage(type,command,channel,message)));
	}
}
