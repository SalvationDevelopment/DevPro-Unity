﻿namespace DevPro.Network.Data
{
	public class SearchRequest
    {
        public int Format { get; set; }
        public int GameType { get; set; }
        public int BanList { get; set; }
        public int TimeLimit { get; set; }
        public bool ActiveGames { get; set; }
        public bool IlligalGames { get; set; }
        public bool Locked { get; set; }
        public string Filter { get; set; }

        public SearchRequest()
        {
            Format = -1;
            GameType = -1;
            BanList = -1;
            TimeLimit = -1;
            ActiveGames = false;
            IlligalGames = false;
            Locked = false;
            Filter = string.Empty;
        }
        public SearchRequest(int format, int type, int banlist, int timelimit, bool active, bool illigal,bool locked,string filter)
        {
            Format = format;
            GameType = type;
            BanList = banlist;
            TimeLimit = timelimit;
            ActiveGames = active;
            IlligalGames = illigal;
            Filter = filter;
            Locked = locked;
        }
    }
}