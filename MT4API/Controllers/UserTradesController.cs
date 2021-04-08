using log4net.Appender;
using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Hosting;
using System.Web.Http;

namespace MT4API.Controllers
{
    [RoutePrefix("v1/api/trades")]
    public class UserTradesController : ApiController
	{
        [Route]
        public IList<TradeRecord> Get()     
        {
            ConnectionParameters connectionparameters = new ConnectionParameters
            { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" };
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";           
            
            var mt4 = new ClrWrapper(connectionparameters, path);            
            mt4.PumpingSwitchEx(default);           
            mt4.PumpingStarted += PumpingStarted;
            Thread.Sleep(6000);
            var trades = mt4.TradesGet();           
            void PumpingStarted(object sender, EventArgs args)
            {
                trades = mt4.TradesGet();
            }
            mt4.Disconnect();           
            return trades;     
        }

        [Route("{groupname}/user/{id}")]
        public IList<TradeRecord> Get(int id,string groupname)
        {
            ConnectionParameters connectionparameters = 
                new ConnectionParameters { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" };
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            var mt4 = new ClrWrapper(connectionparameters, path);
            mt4.PumpingStarted += PumpingStarted;
            mt4.PumpingSwitchEx(default);  
            // if the wifi is not fast.let thread sleep more time be longer;
            Thread.Sleep(6000);
            //string group = "demoforexaugs";
            var trades = mt4.TradesGetByLogin(id, groupname);
            void PumpingStarted(object sender, EventArgs args)
            {
                trades = mt4.TradesGetByLogin(id, groupname);
            }
            mt4.Disconnect();
            return trades;

        }   
    };
}
  