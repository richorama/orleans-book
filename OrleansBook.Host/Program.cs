using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using OrleansBook.GrainClasses;

namespace OrleansBook.Host
{
  class Program
  {
    static async Task Main()
    {
      var host = new SiloHostBuilder()
        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ExampleGrain).Assembly).WithReferences())
        .UseLocalhostClustering()
        .ConfigureLogging(logging =>
        {
          logging.AddConsole();
          logging.SetMinimumLevel(LogLevel.Warning);
        })
        .Build();

      await host.StartAsync();

      Console.WriteLine("Press enter to stop the Silo...");
      Console.ReadLine();

      await host.StopAsync();
    }
  }
}
