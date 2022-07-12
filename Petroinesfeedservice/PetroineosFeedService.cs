using CsvHelper;
using Microsoft.Extensions.Logging;
using Serilog;
using Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Petroineosfeedservice
{
    public class PetroineosFeedService : IPetroinesFeedService
    {
        private ILogger<PetroineosFeedService> _logger;

        public PetroineosFeedService(ILogger<PetroineosFeedService> logger)
        {
            _logger = logger;
        }

        public ReadOnlyCollection<CsvVM> AggregatePowerTrades(IEnumerable<PowerTrade> powerTrades)
        {
            Dictionary<int, double> powerTradeVolumes = new Dictionary<int, double>();

            if (powerTrades != null)
            {
                foreach (var trade in powerTrades)
                {
                    foreach (var period in trade.Periods)
                    {
                        if (!powerTradeVolumes.ContainsKey(period.Period))
                        {
                            powerTradeVolumes.Add(period.Period, period.Volume);
                        }
                        else
                        {
                            powerTradeVolumes[period.Period] += period.Volume;
                        }
                    }
                }
            }
            return powerTradeVolumes.Select(kvp => new CsvVM(kvp.Key, kvp.Value)).ToList().AsReadOnly();
        }

        public void GenerateCsvReport(IEnumerable<CsvVM> powerTradePositions, string reportPath, string reportName)
        {
            var reportFullName = Path.Combine(reportPath, reportName);

            try
            {
                _logger.LogInformation($"Trying to generate the report {reportFullName}");
                using (StreamWriter writer = new StreamWriter(reportFullName))
                {
                    using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csvWriter.Context.RegisterClassMap<CsvExtractorMap>();
                        csvWriter.WriteRecords(powerTradePositions);
                    }
                }
                _logger.LogInformation($"Report {reportFullName} Successfully generated");
            }
            catch (Exception)
            {
                _logger.LogError($"Unable to save the report {reportFullName}");
                _logger.LogError("Report Content below");
                //Todo! write report content
                _logger.LogError(string.Join(Environment.NewLine, powerTradePositions.Select(x => $"{x.LocalTime},{x.Volume}")));

                throw;
            }
        }

        public async Task<ReadOnlyCollection<PowerTrade>> GetPowerTradeAsync()
        {
            int retries = 0, maxRetries = 3;
            _logger.LogInformation("Attempting to Retrieving power trades");
            while (retries < maxRetries)
            {
                try
                {
                    var _powerService = new PowerService();
                    var results = await _powerService.GetTradesAsync(DateTime.Now);
                    _logger.LogInformation($"Power trades successfully retrieved in the {retries + 1} attempt");
                    if (results != null)
                    {
                        var powerTradesReadOnlyList = results.ToList().AsReadOnly();
                        _logger.LogInformation($"Received {powerTradesReadOnlyList.Count} trades");
                        return powerTradesReadOnlyList;
                    }
                }
                catch (PowerServiceException serviceException)
                {
                    _logger.LogError(serviceException.ToString());

                    
                    retries++;

                    if (retries >= maxRetries)
                    {
                        _logger.LogError("Max retries reached");
                    }
                    else
                    {
                        _logger.LogError("Sourcing the data once again after 2 seconds");
                        await Task.Delay(2000);
                    }
                }
            }
            return null;
        }
    }
}