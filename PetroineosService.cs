using Petroineosfeedservice;

namespace PITLReport
{
    public class PetroineosService : IPetroineosService
    {
        private ILogger<PetroineosService> _logger;
        private IPetroinesFeedService _petroinesFeedService;
        
        public PetroineosService(ILogger<PetroineosService> logger, IPetroinesFeedService petroinesFeedService)
        {
            _logger = logger;
            _petroinesFeedService = petroinesFeedService;
        }

        public async Task ExecutePetroineTraderAsync(string reportFilePath)
        {
            var reportName = $"PowerPosition_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            var p1 = await _petroinesFeedService.GetPowerTradeAsync();
            var p2 = _petroinesFeedService.AggregatePowerTrades(p1);
            _petroinesFeedService.GenerateCsvReport(p2, reportFilePath, reportName);
        }
    }

    public interface IPetroineosService
    {
        Task ExecutePetroineTraderAsync(string reportFilePath);
    }
}
