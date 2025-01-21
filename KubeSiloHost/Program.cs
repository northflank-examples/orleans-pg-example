using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace KubeSiloHost;

public static class Program
{
    private static readonly AutoResetEvent Closing = new(false);

    public static async Task<int> Main()
    {
        try
        {
            var builder = new HostBuilder();
            builder.UseOrleans(ConfigureDelegate);
            var host = builder.Build();

            await host.StartAsync();
            Console.WriteLine("Silo is ready!");

            Console.CancelKeyPress += OnExit;
            Closing.WaitOne();

            Console.WriteLine("Shutting down...");

            await host.StopAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 1;
        }
    }

    private static void ConfigureDelegate(HostBuilderContext context, ISiloBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING") 
                               ?? "Host=my_host;Database=my_db;Username=my_user;Password=my_pw";
        var clusterId = Environment.GetEnvironmentVariable("CLUSTER_ID") ?? "testcluster";
        var serviceId = Environment.GetEnvironmentVariable("SERVICE_ID") ?? "testservice";

        builder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = clusterId;
                options.ServiceId = serviceId;
            })
            .UseAdoNetClustering(options =>
            {
                options.Invariant = "Npgsql";
                options.ConnectionString = connectionString;
            })
            .AddAdoNetGrainStorageAsDefault(options =>
            {
                options.Invariant = "Npgsql";
                options.ConnectionString = connectionString;
            })
            .ConfigureLogging(logging => logging.AddConsole());
    }

    private static void OnExit(object sender, ConsoleCancelEventArgs args)
    {
        Closing.Set();
    }
}