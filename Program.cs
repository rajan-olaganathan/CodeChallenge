using PITLReport;
using Petroineosfeedservice;
using Serilog;
using Services;

IHost host = Host.CreateDefaultBuilder(args)
    
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();
        services.AddSingleton<IPowerService, PowerService>();
        services.AddSingleton<IPetroinesFeedService, PetroineosFeedService>();
        services.AddSingleton<IPetroineosService, PetroineosService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
