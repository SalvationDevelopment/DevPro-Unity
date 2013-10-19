using UnityEngine;
using System.Collections;
using DevPro.Network;

public class Login : MonoBehaviour {
	
	Rect m_guiBoxPos = new Rect(Screen.width/2 - 90,Screen.height/2 - 65,180,130);
	string m_username = string.Empty;
	string m_password = string.Empty;
	
	void OnGUI() 
	{
		//GUI.DragWindow();
		GUI.Box(m_guiBoxPos,"Login");
		GUI.Label(GetPosition(new Rect(5,0,100,25)),"Username");
		m_username = GUI.TextField(GetPosition(new Rect(m_guiBoxPos.width - 110,0,100,20)),m_username,15);
		GUI.Label(GetPosition(new Rect(5,35,100,25)),"Password");
		m_password = GUI.PasswordField(GetPosition(new Rect(m_guiBoxPos.width - 110,35,100,20)),m_password,'*',15);
		
		if(GUI.Button(GetPosition(new Rect(m_guiBoxPos.width/2 - 50,65,100,25)),"Login"))
		{
			//handle login
			if(!Main.HubClient.Connected())
			{
				if(!Main.HubClient.Connect(ServerDetails.HubAddress,ServerDetails.HubPort))
				{
					Debug.Log("Failed to connect.");
					//show error of some kind here
					return;
				} else Debug.Log("Connected.");
			}
			
			Main.HubClient.Login(m_username,m_password);
		}
		
	}
	
	Rect GetPosition(Rect item)
	{
		return new Rect(
			m_guiBoxPos.x + item.x,
			m_guiBoxPos.y + 25 + item.y,
			item.width,
			item.height
			);
	}
}
