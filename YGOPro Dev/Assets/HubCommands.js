#pragma strict
import DevPro.Network;
import DevPro.Network.Enums;
import DevPro.Network.Data;
import JsonFx.Json;
import System;
import System.Text;
import System.Security.Cryptography;

private var m_client : HubClient;
private var m_jsonWriter : JsonWriter;
public var User : UserData;

function Start () {
	m_client = new HubClient();
	m_jsonWriter = new JsonWriter();
}

function Update () {
	if(m_client.Connected()) {
		m_client.HandleSendReceive();
		
		while(m_client.HasPacket())
			OnCommand(m_client.GetPacket());
	}
}

function Connect() {
	if(!m_client.Connected())
		m_client.Connect(ServerDetails.HubAddress,ServerDetails.HubPort);
}

function EncodePassword(password : String) : String{
	var salt = Encoding.UTF8.GetBytes("&^%£$Ugdsgs:;");
	var userpassword = Encoding.UTF8.GetBytes(password);
	
	var hmacMD5 = new HMACMD5(salt);
	var saltedHash = hmacMD5.ComputeHash(userpassword);
	
	return Convert.ToBase64String(saltedHash);
}

function OnCommand(data : MessageReceived) {
	Debug.Log(data.Packet);
	
	//handle incoming packets here
}

private var m_lastLoginRequest = new DateTime();
public function Login(username : String, password : String) {
	var delay : TimeSpan;
	delay = DateTime.Now - m_lastLoginRequest;
	if(User != null || delay.TotalMilliseconds < 5000)
		return;
	Connect();
	m_client.SendPacket(DevServerPackets.Login,
		m_jsonWriter.Write(new LoginRequest(username,EncodePassword(password),"Unity")));
	m_lastLoginRequest = DateTime.Now;
}
private var m_lastRegisterRequest  = new DateTime();
public function Register(username : String, password : String) {
	var delay : TimeSpan;
	delay = DateTime.Now - m_lastRegisterRequest;
	if(delay.TotalMilliseconds < 5000)
		return;
	Connect();
	
	m_client.SendPacket(DevServerPackets.Register,
		m_jsonWriter.Write(new LoginRequest(username,EncodePassword(password),"Unity")));
		
	m_lastRegisterRequest = DateTime.Now;
}