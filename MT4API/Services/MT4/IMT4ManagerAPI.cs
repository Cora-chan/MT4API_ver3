using System;
using P23.MetaTrader4.Manager.Contracts;
using P23.MetaTrader4.Manager.Contracts.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT4API.Services.MT4
{
    internal interface IMT4ManagerAPI
    {
        event EventHandler<UserRecord> OnUserUpdate;
        event EventHandler<IList<SymbolInfo>> OnSymbolsUpdate;
        event EventHandler<Group> OnGroupUpdate;
        //event EventHandler<Symbol> OnSymbolUpdate;
        event EventHandler<TradeRecord> OnTradeAdded;
        event EventHandler<TradeRecord> OnTradeDeleted;
        event EventHandler<TradeRecord> OnTradeClosed;
        event EventHandler<TradeRecord> OnTradeUpdated;
        event EventHandler<MarginLevel> OnMarginCallUpdated;
        event EventHandler OnPumpingStarted;
        //event EventHandler OnBidAskUpdated;

        IList<TradeRecord> TradesGet();
        IList<TradeRecord> TradesUserHistory(int login, uint from, uint to);
        IList<SymbolInfo> FetchSymbolInfo();
        IList<Symbol> SymbolsRefreshAndGetSymbols();
        IList<UserRecord> GetAllUsers();
        IList<Group> GetAllUserGroups();
        IList<SymbolSummary> SummaryGetAll();
        IList<TickRecord> TicksRequest(TickRequest request);
        IList<RateInfo> ChartRequest(ChartInfo chart);
        IList<DailyReport> DailyReportsRequest(DailyGroupRequest request, IList<int> logins);
        int UpdateSymbol(Symbol symbol);
        // event EventHandler<LoginPosition> OnPositionUpdated;
        UserRecord UserRecordGet(int login);

        void StartPumping();
        void StartPumpingSymbol();
        void StartNormal();
        void Stop();
        void Disconnect();
        void Connect();
    }
}
