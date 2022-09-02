using Domain.DataAccess;
using Domain.Services.Contracts;
using Domain.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        try
        {
            var mainService = host.Services.GetService<IMainService>();
            await mainService.DoMainOperation();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

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
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(config["ConnectionStrings:Default"]);
                    //options.EnableThreadSafetyChecks(false);
                    options.EnableDetailedErrors(true);
                    options.EnableSensitiveDataLogging(false);
                }, ServiceLifetime.Transient);

                services.AddTransient<IMainService, MainService>();
                services.AddScoped(typeof(IExcelReader<>), typeof(ExcelReader<>));
                services.AddTransient<IExcelGeneratorService, ExcelGeneratorService>();

                services.AddStackExchangeRedisCache(options =>
                {
                    options.InstanceName = "BulkOp_";
                    options.Configuration = config.GetConnectionString("Redis");
                });
            });

        return hostBuilder;
    }
}