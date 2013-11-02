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
			Application.ExternalCall("MessageBrowser", message);
		}
	}
}

