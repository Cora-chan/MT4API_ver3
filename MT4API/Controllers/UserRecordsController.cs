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
using MT4API.Model;
using MT4API.Utils;
using System.Runtime.InteropServices;
using System.Threading;

namespace MT4API.Controllers
{
    public class UserRecordsController : ApiController
    {
        public UserRecord Get(int id)
        {
            //add absolute path to path to mtmanapi.dll in bin
            string path = @"D:\Yue's Project\MT4API_July_27th\MT4API_ver3\MT4API\bin\mtmanapi.dll";
            var are = new AutoResetEvent(false);
            using (var mt = new ClrWrapper(new ConnectionParameters
            {
                Login = 538,
                Password = "mvW9twXd",
                Server = "192.149.48.62:443"
            }, path))

            {
                mt.PumpingSwitch(i =>
                {
                    if (i == 0) // 0 - means pumping started
                        are.Set();
                });

                are.WaitOne();
                var traderecord = mt.UserRecordGet(id);
                return traderecord;
            }
        }
    }
}
