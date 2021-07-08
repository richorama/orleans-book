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
          var grain = client.GetGrain<IRobotGrain>(grainId);
          
          
          Console.WriteLine("Please enter an instruction value...");
          var value = Console.ReadLine();
          await grain.AddInstruction(value);

          var count = await grain.GetInstructionCount();
          Console.WriteLine($"Instruction count = {count}");
        }
      }

    }
  }
}
