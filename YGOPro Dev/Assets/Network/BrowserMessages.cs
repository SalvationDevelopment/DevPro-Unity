using System;
using UnityEngine;
using DevPro.Network.Data;
using Pathfinding.Serialization.JsonFx;

namespace DevPro.Network
{
	public static class BrowserMessages
	{
		public static void SendConsoleMessage(string message)
		{
			Application.ExternalCall("ConsoleMessage", message);
		}
		
		public static void SendMessage(string message)
		{
			Application.ExternalCall("SayHello", message);
		}
	}
}

