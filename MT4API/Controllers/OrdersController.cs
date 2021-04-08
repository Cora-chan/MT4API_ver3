using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using P23.MetaTrader4.Manager.Contracts.Configuration;
using System.Web;
using MT4API;
using MT4API.Services;
using MT4API.Model;
using MT4API.Utils;
using System.Runtime.InteropServices;
using System.Threading;
using MT4API.Services.MT4;
using System.Web.UI.WebControls;

namespace MT4API.Controllers
{
    [RoutePrefix("v1/api/trades")]
    public class OrdersController : ApiController
    {
        
        public IList<TradeRecord> Get()
        {
            ConnectionParameters connectionparameters = new ConnectionParameters { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" };
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            var mt4 = new ClrWrapper(connectionparameters,path);
            mt4.PumpingStarted += PumpingStarted;
            mt4.PumpingSwitchEx(default);
            Thread.Sleep(6000);
            var trades = mt4.TradesGet();
            void PumpingStarted(object sender, EventArgs args)
            {
                 trades = mt4.TradesGet();
            }
            mt4.Disconnect();


            return trades;
        }

        [Route("order/{id}")]
        public TradeRecord Get(int id)
        {
            ConnectionParameters connectionparameters = new ConnectionParameters { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" };
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            var mt4 = new ClrWrapper(connectionparameters, path);
            mt4.PumpingStarted += PumpingStarted;
            mt4.PumpingSwitchEx(default);
            Thread.Sleep(6000);
            var trade = mt4.TradeRecordGet(id);
            void PumpingStarted(object sender, EventArgs args)
            {
                trade = mt4.TradeRecordGet(id);
            }
            // The disconnect is compulsary here because it has to stop the threads. Otherwise there will be appdomain exception
            mt4.Disconnect();
            return trade;

        }

        public HttpResponseMessage Post(TradeTransInfo tradeTransInfo)
        {
            //add absolute path to path to mtmanapi.dll in bin
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            using (var mt = new ClrWrapper
                (new ConnectionParameters
                { Login = 538, Password = "mvW9twXd", Server = "192.149.48.62:443" }, path))
            {
                //var tradetransinfo = new TradeTransInfo
                //{
                //    Type = TradeTransactionType.BrOrderOpen,
                //    Cmd = TradeCommand.Buy,
                //    Comment = "try in controller22233333",
                //    Symbol = "AUDNZD-S",
                //    Volume = 1,
                //    OrderBy = 99999994,
                //    Price = 1.07684,
                //    Order = 0,
                //    Tp = 0,
                //    Sl = 0,
                //};
                var result = mt.TradeTransaction(tradeTransInfo);

                return Request.CreateResponse(HttpStatusCode.Created, tradeTransInfo);
            }
        }
    }
}

