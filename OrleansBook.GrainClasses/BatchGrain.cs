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

    public Task Put(WithKey<StorageValue>[] values)
    {
      var tasks = values.Select(keyValue => 
        _client
          .GetGrain<IKeyValueGrain>(keyValue.Key)
          .Put(keyValue.Value.Value)
      );

      return Task.WhenAll(tasks);
    }
  }
}

