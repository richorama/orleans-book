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

    [Transaction(TransactionOption.Create)]
    public Task Put(WithKey<StorageValue>[] values)
    {
      var tasks = values.Select(keyValue => 
        _client
          .GetGrain<ICacheGrain<StorageValue>>(keyValue.Key, "StorageValueGrain")
          .Put(keyValue.Value)
      );

      return Task.WhenAll(tasks);
    }
  }
}

