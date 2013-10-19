using UnityEngine;
using System.Collections;
using DevPro.Network;
using DevPro.Network.Enums;
using DevPro.Network.Data;

public class HubHandler : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Main.HubClient.Connected())
		{
			Main.HubClient.HandleSendReceive();
			
			while(Main.HubClient.HasPacket())
				OnCommand(Main.HubClient.GetPacket());
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
		default:
			break;
		}
	}
}
