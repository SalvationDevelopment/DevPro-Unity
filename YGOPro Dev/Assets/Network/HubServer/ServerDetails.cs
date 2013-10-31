using System;
using System.Collections.Generic;
using DevPro.Network.Data;

namespace DevPro.Network
{
	public static class ServerDetails
	{
		public static Dictionary<string, ServerInfo> ServerList = new Dictionary<string, ServerInfo>();
		public static string HubAddress = "91.250.87.52";
		public static int HubPort = 8933;
		public static UserData User;
		public static int LoginKey;
		private static Random Random = new Random();
		
		public static ServerInfo GetServer(string name)
		{
			if(ServerList.ContainsKey(name))
				return ServerList[name];
			return null;
		}
		
		public static ServerInfo GetRandomServer()
		{
			if(ServerList.Count == 0)
				return null;
			
			List<string> keys = new List<string>(ServerList.Keys);
			int key = Random.Next(0,keys.Count);
			
			return ServerList[keys[key]];
		}
	}
}

