using System;
namespace DevPro.Network.Data
{
	public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string UID { get; set; }
		
		public LoginRequest(string username, string password, string uid)
		{
			Username = username;
			Password = password;
			UID = uid;
		}
    }
}
