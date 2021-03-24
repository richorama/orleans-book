using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Transactions.Abstractions;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class KeyValueGrain : Grain, IKeyValueGrain
  {
    ITransactionalState<StorageValue> _value;

    public KeyValueGrain(
      [TransactionalState("value", "TransactionStore")]
      ITransactionalState<StorageValue> value)
    {
      this._value = value;
    }

    public Task Put(string value) => 
      _value.PerformUpdate(x => x.Value = value);

    Task<string> IKeyValueGrain.Get() =>
      _value.PerformRead(x => x.Value);
  }
}

