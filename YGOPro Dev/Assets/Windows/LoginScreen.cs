using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using DevPro.Network;

public class LoginScreen : MonoBehaviour {
	//login values
	Rect m_loginRect = new Rect(Screen.width/2 - 90,Screen.height/2 - 65,180,130);
	bool m_showLogin = true;
	string m_loginUsername = string.Empty;
	string m_loginPassword = string.Empty;
	//register values
	Rect m_registerRect = new Rect(Screen.width/2 - 90,Screen.height/2 - 65,180,130);
	bool m_showRegister = false;
	string m_registerUsername = string.Empty;
	string m_registerPassword = string.Empty;
	string m_registerConfirm = string.Empty;
	
	void OnGUI() 
	{
		if(m_showLogin)
			m_loginRect = GUI.Window(0,m_loginRect,DrawLoginWindow,"YGOPro Dev - Login");
		if(m_showRegister)
			m_registerRect = GUI.Window(1,m_registerRect,DrawRegisterWindow,"Register");
	}
	
	void DrawLoginWindow(int id)
	{
		GUI.Label(new Rect(5,25,100,25),"Username");
		m_loginUsername = GUI.TextField(new Rect(m_loginRect.width - 110,25,100,20),m_loginUsername,15);
		GUI.Label(new Rect(5,60,100,25),"Password");
		m_loginPassword = GUI.PasswordField(new Rect(m_loginRect.width - 110,60,100,20),m_loginPassword,'*',15);
		
		if(GUI.Button(new Rect(20,90,70,25),"Login"))	
			Main.Game.Login(m_loginUsername,m_loginPassword);
		
		if(GUI.Button(new Rect(95,90,70,25),"Register"))
		{
			m_showLogin = false;
			m_showRegister = true;
		}
		
		//make dragable
		GUI.DragWindow(new Rect(0, 0, m_loginRect.width, 20));
	}
		
	void DrawRegisterWindow(int id)
	{
		GUI.Label(new Rect(5,25,100,25),"Username");
		m_registerUsername = GUI.TextField(new Rect(m_registerRect.width - 110,25,100,20),m_registerUsername,15);
		GUI.Label(new Rect(5,50,100,25),"Password");
		m_registerPassword = GUI.PasswordField(new Rect(m_registerRect.width - 110,50,100,20),m_registerPassword,'*',15);
		GUI.Label(new Rect(5,75,100,25),"Confirm");
		m_registerConfirm = GUI.PasswordField(new Rect(m_registerRect.width - 110,75,100,20),m_registerConfirm,'*',15);
		
		if(GUI.Button(new Rect(20,100,70,25),"Register"))
			Register();
		
		if(GUI.Button(new Rect(95,100,70,25),"Close"))
		{
			m_showRegister = false;
			m_showLogin = true;
		}
		
		//make dragable
		GUI.DragWindow(new Rect(0,0,m_registerRect.width,20));
	}
	
	void Register()
	{
		if(string.IsNullOrEmpty(m_registerUsername))
		{
			Debug.Log("Please enter username.");
			return;
		}
		
		if(string.IsNullOrEmpty(m_registerPassword))
		{
			Debug.Log("Please enter password.");
			return;
		}
		
		if(m_registerPassword != m_registerConfirm)
		{
			Debug.Log("Invalid password check.");
			return;
		}
		if (!Regex.IsMatch(m_registerUsername, "^[a-zA-Z0-9_]*$"))
        {
			Debug.Log("Username contains invalid characters.");
			return;
        }
		Main.Game.Register(m_registerUsername,m_registerPassword);
	}
}
