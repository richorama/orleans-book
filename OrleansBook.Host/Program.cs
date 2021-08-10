using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Statistics;
using OrleansBook.GrainClasses;

namespace OrleansBook.Host
{
  class Program
  {
    static async Task Main()
    {
      var host = new SiloHostBuilder()
        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(RobotGrain).Assembly).WithReferences())
        .UseLocalhostClustering()
        .ConfigureLogging(logging =>
        {
          logging.AddConsole();
          logging.SetMinimumLevel(LogLevel.Information);
        })
        .UseDashboard()
        .UseLinuxEnvironmentStatistics()
        .AddMemoryGrainStorage("robotStateStore")
        .AddMemoryGrainStorage("PubSubStore")
        .AddSimpleMessageStreamProvider("SMSProvider")
        .UseInMemoryReminderService()
        .AddAzureTableTransactionalStateStorage("TransactionStore", o => {
          o.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=two10ra;AccountKey=dmIMUY1mg/qPeOgGmCkO333L26cNcnUA1uMcSSOFMB3cB8LkdDkh02RaYTPLBL8qMqnqazqd6uMxI2bJJEnj0g==";
        })
        .UseTransactions()
        .Build();

      await host.StartAsync();

      Console.WriteLine("Press enter to stop the Silo...");
      Console.ReadLine();

      await host.StopAsync();
    }
  }
}
