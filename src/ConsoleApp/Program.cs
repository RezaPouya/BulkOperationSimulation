using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Operator.Models;
using Operator.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var mainService = host.Services.GetService<IMainService>();

        await mainService.DoMainOperation();

        Console.Read();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        var config = builder.Build();

        var hostBuilder = Host.CreateDefaultBuilder(args)

            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(Directory.GetCurrentDirectory());
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContextPool<AppDbContext>(options =>
                {
                    options.UseSqlServer(config["ConnectionStrings:Default"]);
                    options.EnableThreadSafetyChecks(false);
                    options.EnableDetailedErrors(true);
                    options.EnableSensitiveDataLogging(false);
                });
                services.AddTransient<IMainService, MainService>();
                services.AddScoped(typeof(IExcelReader<>), typeof(ExcelReader<>));
                services.AddTransient<IExcelGeneratorService, ExcelGeneratorService>();
                services.AddStackExchangeRedisCache(options =>
                {
                    options.InstanceName = config["Redis:InstanceName"];
                    options.Configuration = config["Redis:Configuration"];
                });
            });

        return hostBuilder;
    }
}