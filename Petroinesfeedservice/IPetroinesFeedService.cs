using Services;
using System.Collections.ObjectModel;

namespace Petroineosfeedservice
{
    public interface IPetroinesFeedService
    {
        Task<ReadOnlyCollection<PowerTrade>> GetPowerTradeAsync();
        ReadOnlyCollection<CsvVM> AggregatePowerTrades(IEnumerable<PowerTrade> powerTrades);
        void GenerateCsvReport(IEnumerable<CsvVM> powerTradePositions, string reportPath, string reportName);
    }
}