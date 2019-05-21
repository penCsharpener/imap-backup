using System;
using System.Collections.Generic;
using System.Text;

namespace penCsharpener.ImapData {
    public class DbCredentials {

        public DbCredentials(string serverURL, ushort port, string username, string password) {
            ServerURL = serverURL;
            Port = port;
            Username = username;
            Password = password;
        }

        public string ServerURL { get; set; }
        public ushort Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public override string ToString() {
            return $"Server={ServerURL};Port={Port};Uid={Username};Pwd={Password};ConvertZeroDateTime=True;";
        }
    }
}
