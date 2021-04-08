using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace MT4API.Controllers
{
	[RoutePrefix("v1/api/trades")]
	public class GroupTradesController : ApiController
    {
		//name must be lowercase
		[Route("{groupname}")]
		public Dictionary<int, TradeRecord> Get(string groupname)
        {
			ConnectionParameters connectionparameters = new ConnectionParameters { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" };
			string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
			var mt4 = new ClrWrapper(connectionparameters, path);
			mt4.PumpingStarted += PumpingStarted;
			mt4.PumpingSwitchEx(default);
			Thread.Sleep(4000);

			var trades = mt4.TradesGet();
			var users = mt4.UsersGet();

			Dictionary<int, UserRecord> dictionaryusers = new Dictionary<int, UserRecord>();
			Dictionary<int, TradeRecord> dictionarytrades = new Dictionary<int, TradeRecord>();
			Dictionary<int, TradeRecord> resulttrades = new Dictionary<int, TradeRecord>();

			List<int> loginlist = new List<int>();

			foreach (var user in users)
			{
				dictionaryusers.Add(user.Login, user);

				if (user.Group == groupname)
				{
					loginlist.Add(user.Login);
				}

			}

			foreach (var trade in trades)
			{
				dictionarytrades.Add(trade.Order, trade);

				foreach (var element in loginlist)
				{
					if (element == trade.Login)
					{
						resulttrades.Add(trade.Order, trade);
					}
				}
			}

			void PumpingStarted(object sender, EventArgs args)
			{
				trades = mt4.TradesGet();
			}

			mt4.Disconnect();
			return resulttrades;
		}
	}
}
    
