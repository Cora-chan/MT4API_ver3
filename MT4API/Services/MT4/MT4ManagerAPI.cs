using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;
using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using P23.MetaTrader4.Manager.Contracts.Configuration;
using MT4API.Model;
using MT4API.Utils;

namespace MT4API.Services.MT4
{
    public class MT4ManagerAPI : IMT4ManagerAPI
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _host;
        private readonly int _login;
        private readonly string _password;
        private bool _isStopRequested = false;
        private bool bCheckConnectionFlag = false;
        private ClrWrapper manager;
        int connectCount = 0;

        // Timer
        private static System.Timers.Timer timer;

        public event EventHandler<UserRecord> OnUserUpdate;
        public event EventHandler<IList<SymbolInfo>> OnSymbolsUpdate;
        public event EventHandler<Group> OnGroupUpdate;
        public event EventHandler<Symbol> OnSymbolUpdate;
        public event EventHandler<TradeRecord> OnTradeAdded;
        public event EventHandler<TradeRecord> OnTradeDeleted;
        public event EventHandler<TradeRecord> OnTradeClosed;
        public event EventHandler<TradeRecord> OnTradeUpdated;
        public event EventHandler<MarginLevel> OnMarginCallUpdated;
        public event EventHandler OnPumpingStarted;
        // public event EventHandler<LoginPosition> OnPositionUpdated;

        public Dictionary<int, TradeRecord> OpenTrades = new Dictionary<int, TradeRecord>();

        // public Dictionary<int, Dictionary<string, LoginPosition>> PositionBySymbolByLogin;

        public MT4ManagerAPI(bool bLive = true)
        {

            Secret secret = SecretUtil.GetLiveHostSecret();
            if (!bLive)
                secret = SecretUtil.GetDemoHostSecret();
            this._host = secret.Host;
            this._login = secret.Login;
            this._password = secret.Password;
            manager = new ClrWrapper();
        }

        public MT4ManagerAPI(string host, int login, string password)
        {
            this._host = host;
            this._login = login;
            this._password = password;

            manager = new ClrWrapper();

        }

        public MT4ManagerAPI(Secret secret)
        {
            this._host = secret.Host;
            this._login = secret.Login;
            this._password = secret.Password;

            manager = new ClrWrapper();
        }

        // Start starts ClrWrapper and enters extended pumping mode
        public void StartPumping()
        {
            _isStopRequested = false;
            Connect();
            SwitchToPumping();
            SetupTimer();
        }

        public void StartNormal()
        {
            _isStopRequested = false;
            Connect();
            InitSymbols();
            SetupTimer();
        }

        public void StartPumpingSymbol()
        {
            _isStopRequested = false;
            Connect();
            SwitchToPumpingWithSymbol();
            SetupTimer();
        }

        // Connect establish connections and login to mt4 server
        public void Connect()
        {
            bCheckConnectionFlag = true;
            bool emailFlag = false;
            while (manager.IsConnected() != 1)
            {
                connectCount++;
                if (connectCount % 6 == 0)
                {
                    //if (!emailFlag)
                    //{
                    //    EmailService emailService = new EmailService();
                    //    emailService.SendEmailAlarm("Data Manager | Service Lost Connection", string.Format("Reconnect Time {0}", connectCount));
                    //    emailFlag = true;
                    //}

                    //Npgsql.NpgsqlConnection.ClearAllPools();
                }
                var connRet = manager.Connect(_host);
                var loginRet = manager.Login(_login, _password);
                Thread.Sleep(1000 * 5);
            }
            bCheckConnectionFlag = false;
        }
        public void Disconnect()
        {
            manager.Disconnect();
        }

        // SwitchToPumping switches ClrWrapper connection from normal mode to pumping mode
        private void SwitchToPumping()
        {
            InitSymbols();

            // attach event handler here
            var are = AttachEventHandlers();
            manager.PumpingSwitchEx(PumpingMode.Default);

            if (!are.WaitOne(TimeSpan.FromSeconds(20)))
            {
                manager.Disconnect();
                //MainController.Instance.StartDaily();
                //System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
                //Environment.Exit(0);
            }

        }

        // SwitchToPumping switches ClrWrapper connection from normal mode to pumping mode
        private void SwitchToPumpingWithSymbol()
        {
            InitSymbols();
            manager.PumpingSwitchEx(PumpingMode.Default);
        }

        public IList<Symbol> SymbolsRefreshAndGetSymbols()
        {
            Connect();
            manager.SymbolsRefresh();
            var allSymbols = manager.SymbolsGetAll();
            return allSymbols;
        }

        public IList<UserRecord> GetAllUsers()
        {
            Connect();
            var allUsers = manager.UsersRequest();

            return allUsers; 
        }
        public IList<Group> GetAllUserGroups()
        {
            Connect();
            var userGroups = manager.GroupsRequest();
            return userGroups;
        }
        public UserRecord UserRecordGet(int login)
        {
            var user = manager.UserRecordGet(login);
            return user;
        }
        public IList<SymbolSummary> SummaryGetAll()
        {
            var allSummaries = manager.SummaryGetAll();
            return allSummaries;
        }

        // AttachEventHandlers return a AutoResetEvent to be signaled by PumpingStarted Event
        private AutoResetEvent AttachEventHandlers()
        {
            manager.BidAskUpdated += BidAskUpdated;
            manager.GroupUpdated += GroupUpdated;
            manager.SymbolUpdated += SymbolUpdated;
            manager.TradeAdded += TradeAdded;
            manager.TradeClosed += TradeClosed;
            manager.TradeDeleted += TradeDeleted;
            manager.TradeUpdated += TradeUpdated;
            manager.UserUpdated += UserUpdated;
            manager.MarginCallUpdated += MarginCallUpdated;
            manager.PumpingStopped += PumpingStopped;

            var are = new AutoResetEvent(false);
            manager.PumpingStarted += (s, e) => PumpingStarted(s, e, are);
            return are;
        }

        private void PumpingStarted(ClrWrapper sender, EventArgs eventArgs, AutoResetEvent are)
        {
            are.Set();

            try
            {
                OnPumpingStarted?.Invoke(this, null);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void PumpingStopped(ClrWrapper sender, EventArgs eventArgs)
        {
            if (_isStopRequested)
            {
                return;
            }
            Task.Delay(TimeSpan.FromSeconds(2)).ContinueWith((t) =>
            {
                StartPumping();
            });
        }

        private void MarginCallUpdated(ClrWrapper sender, EventArgs eventArgs)
        {
            var margins = manager.MarginsGet();
            foreach (var margin in margins)
            {
                OnMarginCallUpdated?.Invoke(this, margin);
            }
        }

        private void InitSymbols()
        {
            manager.SymbolsRefresh();
            var symbols = manager.SymbolsGetAll();
            foreach (var symbol in symbols)
            {
                manager.SymbolAdd(symbol.Name);
                SymbolUpdated(null, symbol);
            }
        }


        private void InitUsers()
        {
            var users = manager.UsersGet();
            foreach (var user in users)
            {
                OnUserUpdate?.Invoke(this, user);
            }
        }

        private void InitGroups()
        {
            var groups = manager.GroupsGet();
            foreach (var group in groups)
            {
                OnGroupUpdate?.Invoke(this, group);
            }
        }

        private void InitTrades()
        {
            var trades = manager.TradesGet();
            foreach (var trade in trades)
            {
                OnTradeUpdated?.Invoke(this, trade);
            }
        }

        private void UserUpdated(ClrWrapper sender, P23.MetaTrader4.Manager.Contracts.UserRecord userRecord)
        {
            try
            {
                OnUserUpdate?.Invoke(this, userRecord);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public IList<TradeRecord> TradesGet()
        {
            try
            {
                IList<TradeRecord> tradesList = manager.TradesGet();
                return tradesList;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IList<TradeRecord> TradesUserHistory(int login, uint from, uint to)
        {
            try
            {
                IList<TradeRecord> tradesList = manager.TradesUserHistory(login, from, to);
                return tradesList;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        public IList<SymbolInfo> FetchSymbolInfo()
        {
            try
            {
                IList<SymbolInfo> symbols = manager.SymbolInfoUpdated();
                return symbols;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }
        public IList<TickRecord> TicksRequest(TickRequest request)
        {
            return manager.TicksRequest(request);
        }

        public IList<RateInfo> ChartRequest(ChartInfo chart)
        {
            var ratesInfoList = manager.ChartRequest(chart, out uint timesign);
            return ratesInfoList;
        }
        public IList<DailyReport> DailyReportsRequest(DailyGroupRequest request, IList<int> logins)
        {
            Connect();
            var dailyReports = manager.DailyReportsRequest(request, logins);
            return dailyReports;
        }
        public int UpdateSymbol(Symbol symbol)
        {
            manager.CfgUpdateSymbol(symbol);
            return 0;
        }
        private void TradeUpdated(ClrWrapper sender, TradeRecord tradeRecord)
        {
            try
            {
                OnTradeUpdated?.Invoke(this, tradeRecord);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void TradeDeleted(ClrWrapper sender, TradeRecord tradeRecord)
        {
            try
            {
                OnTradeDeleted?.Invoke(this, tradeRecord);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void TradeClosed(ClrWrapper sender, TradeRecord tradeRecord)
        {
            try
            {
                OnTradeClosed?.Invoke(this, tradeRecord);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void TradeAdded(ClrWrapper sender, TradeRecord tradeRecord)
        {
            try
            {
                OnTradeAdded?.Invoke(this, tradeRecord);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }



        private void SymbolUpdated(ClrWrapper sender, Symbol symbolConfiguration)
        {
            try
            {
                OnSymbolUpdate?.Invoke(this, symbolConfiguration);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void GroupUpdated(ClrWrapper sender, Group groupConfiguration)
        {
            try
            {
                OnGroupUpdate?.Invoke(this, groupConfiguration);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void BidAskUpdated(ClrWrapper sender, EventArgs eventArgs)
        {
            try
            {
                IList<SymbolInfo> symbols = manager.SymbolInfoUpdated();

                OnSymbolsUpdate?.Invoke(this, symbols);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void Stop()
        {
            _isStopRequested = true;
            if (manager != null)
            {
                manager.Dispose();
            }
        }


        private void SetupTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 100.0 * 5;
            timer.Elapsed += HoodConnectEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void HoodConnectEvent(Object source, ElapsedEventArgs e)
        {
            if (!bCheckConnectionFlag)
                Connect();
        }
    }
}
