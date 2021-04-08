using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT4API.Model
{
    public class Secret
    {
        private string _username;
        public string Username
        {
            get
            {
                return this._username;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this._username = "UNKNOW";
                else
                    this._username = value;
            }
        }

        private int _login;
        public int Login
        {
            get
            {
                return this._login;
            }
            set
            {
                this._login = value;
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this._password = "UNKNOW";
                else
                    this._password = value;
            }
        }

        private string _host;
        public string Host
        {
            get
            {
                return this._host;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this._host = "UNKNOW";
                else
                    this._host = value;
            }
        }
        public string RedisServer { get; set; }

        public Secret(string redisServer)
        {
            RedisServer = redisServer;
        }

        public Secret(string username, string password, string host)
        {
            this.Username = username;
            this.Password = password;
            this.Host = host;
        }

        public Secret(int login, string password, string host)
        {
            this.Login = login;
            this.Password = password;
            this.Host = host;
        }

    }
}
