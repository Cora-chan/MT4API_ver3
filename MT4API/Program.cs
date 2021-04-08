using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MT4API.Services.MT4;
using System.Threading;
using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using P23.MetaTrader4.Manager.Contracts.Configuration;
using MT4API.Model;
using MT4API.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using System.Configuration;

namespace MT4API
{
	public class Program
	{
		
		static void Main()
		{
			

			ConnectionParameters connectionparameters = new ConnectionParameters { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" };
			string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
			var mt4 = new ClrWrapper(connectionparameters, path);
			mt4.PumpingStarted += PumpingStarted;
			mt4.PumpingSwitchEx(default);
			Thread.Sleep(2000);
			
			var trades = mt4.TradesGet();
			var users = mt4.UsersGet();

			Dictionary<int, UserRecord> dictionaryusers = new Dictionary<int, UserRecord>();
			Dictionary<int, TradeRecord> dictionarytrades = new Dictionary<int, TradeRecord>();
			Dictionary<int, TradeRecord> resulttrades = new Dictionary<int, TradeRecord>();

			List<int> loginlist = new List<int>();

            foreach (var user in users)
            {
                dictionaryusers.Add(user.Login, user);

				if (user.Group == "demoforexaugs")
				{
					loginlist.Add(user.Login);
				}

            }

			foreach (var trade in trades)
            {
				dictionarytrades.Add(trade.Order, trade);

				foreach (var element in loginlist)
                {
					if (element ==trade.Login)
                    {
						resulttrades.Add(trade.Order, trade);
                    }
                }
            }
			

		
			void PumpingStarted(object sender, EventArgs args)
			{
				trades = mt4.TradesGet();
			}
		}	
	}
}




