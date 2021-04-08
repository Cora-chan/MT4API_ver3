using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using P23.MetaTrader4.Manager.Contracts;
using P23.MetaTrader4.Manager;

namespace MT4API.Services.MT4
{
    public class MT4Service
    {
        private static readonly object padlock;
        private static MT4Service _instance;
        public static MT4Service Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MT4Service();
                    }
                    return _instance;
                }
            }
        }
        //
        //
        //
        private static IMT4ManagerAPI mt4API;
        public MT4Service()
        {
            mt4API = new MT4ManagerAPI();
        }
    }
}