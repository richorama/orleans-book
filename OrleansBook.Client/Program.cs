using System;
using System.Threading.Tasks;
using Orleans;
using Microsoft.Extensions.Logging;
using OrleansBook.GrainInterfaces;
using Orleans.Hosting;

namespace OrleansBook.Client
{

  class Program
  {
    static async Task Main(string[] args)
    {
      var client = new ClientBuilder()
        .UseLocalhostClustering()
        .AddSimpleMessageStreamProvider("SMSProvider")
        .Build();

      using (client)
      {
        await client.Connect();

        while (true)
        {
          Console.WriteLine("Please enter a Grain ID...");
          var grainId = Console.ReadLine();
          var grain = client.GetGrain<ICacheGrain<StorageValue>>(grainId);
          
          var currentValue = await grain.Get();
          Console.WriteLine($"Grain {grainId} = {currentValue?.Value ?? "null"}");

          Console.WriteLine("Please enter a value...");
          var value = Console.ReadLine();
          await grain.Put(new StorageValue { Value = value });

          currentValue = await grain.Get();
          Console.WriteLine($"Grain {grainId} = {currentValue.Value ?? "null"}");
        }
      }

    }
  }
}
