using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using HelloWorld.Interfaces;

namespace KubeClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                // Configure logging at the Host level
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // Optional: Clears default logging providers
                    logging.AddConsole();     // Adds Console logging
                    // You can add more logging providers here if needed
                })
                .ConfigureServices(services =>
                {
                    services.AddOrleansClient(clientBuilder =>
                    {
                        clientBuilder.UseAdoNetClustering(options =>
                        {
                            options.Invariant = "Npgsql";
                            options.ConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
                                ?? "Host=my_host;Database=my_db;Username=my_user;Password=my_pw";
                        })
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = Environment.GetEnvironmentVariable("CLUSTER_ID") ?? "testcluster";
                            options.ServiceId = Environment.GetEnvironmentVariable("SERVICE_ID") ?? "testservice";
                        });
                        // Removed clientBuilder.ConfigureLogging
                    });
                })
                .Build();

            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            try
            {
                await host.StartAsync(cancellationTokenSource.Token);
                var client = host.Services.GetRequiredService<IClusterClient>();

                Console.WriteLine("Client successfully connected to silo host");

                await DoClientWork(client);

                // Wait indefinitely until cancellation is requested
                await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                await host.StopAsync();
            }
        }

        private static async Task DoClientWork(IGrainFactory client)
        {
            var friend = client.GetGrain<IHello>(0);
            for (var i = 0; i < 10; i++)
            {
                var response = await friend.SayHello("Good morning, my friend!");
                Console.WriteLine($"\n\n{response}\n\n");
            }
        }
    }
}