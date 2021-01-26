using System;
using System.Threading.Tasks;
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
        .Build();
      
      await host.StartAsync();

      Console.WriteLine("Press enter to stop the Silo...");
      Console.ReadLine();

      await host.StopAsync();
    }
  }
}
