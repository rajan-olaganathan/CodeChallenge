using Serilog;
namespace PITLReport
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IPetroineosService _petroinesService  ;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IPetroineosService petroinesService)
        {
            _logger = logger;
            _configuration = configuration;
            _petroinesService = petroinesService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var intervalMinutes = _configuration.GetValue(typeof(string), "appsettings:intervalFrequency");
                var reportFilePath = _configuration.GetValue(typeof(string), "appsettings:reportFilePath");
                await _petroinesService.ExecutePetroineTraderAsync((string)reportFilePath);
                await Task.Delay(TimeSpan.FromMinutes(int.Parse((string)intervalMinutes)), stoppingToken);
            }
        }

        
    }
}