using System;
using UnityEngine;
using DevPro.Game;
using DevPro.Network.Data;
using Pathfinding.Serialization.JsonFx;

namespace DevPro.Network
{
	public static class BrowserMessages
	{
		public static void IsLoaded()
		{
			Application.ExternalCall("IsLoaded");	
		}
		
		public static void MessagePopUp(string message)
		{
			Application.ExternalCall("MessagePopUp", message);
		}
#region Launcher
		public static void LoginAccept(string username)
		{
			Application.ExternalCall("LoginAccept",username);	
		}
#endregion
#region Lobby
		
		public static void RoomInfo(RoomInfo info)
		{
			Application.ExternalCall("SetRoomInfo",JsonWriter.Serialize(info));
		}
		
		//This Refers the you the player and not other players
		public static void PositionUpdate(int pos)
		{
			Application.ExternalCall("PosUpdate",pos);
		}
		
		public static void PlayerEnter(string username,int pos)
		{
			Application.ExternalCall("PlayerEnter",username,pos);
		}
		
		public static void PlayerLeave(int pos)
		{
			Application.ExternalCall("PlayerLeave",pos);
		}
		
		public static void UpdatePlayer(int pos, int newpos)
		{
			Application.ExternalCall("UpdatePlayer",pos,newpos);
		}
		
		public static void PlayerReady(int pos, bool ready)
		{
			Application.ExternalCall("PlayerReady",pos,ready);
		}
		
#endregion
#region Duel
		
#endregion
	}
}

