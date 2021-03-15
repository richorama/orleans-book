using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{

  [StatelessWorker]
  public class BatchGrain : Grain, IBatchGrain
  {
    IClusterClient _client;

    public BatchGrain(IClusterClient client) =>
      _client = client;

    public Task Put((string,string)[] values)
    {
      var tasks = values.Select(keyValue => 
        _client
          .GetGrain<IKeyValueGrain>(keyValue.Item1)
          .Put(keyValue.Item2)
      );

      return Task.WhenAll(tasks);
    }
  }
}

