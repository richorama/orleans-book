using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Transactions.Abstractions;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class StorageValueGrain : Grain, ICacheGrain<StorageValue>
  {
    ITransactionalState<StorageValue> _value;

    public StorageValueGrain(
      [TransactionalState("value", "TransactionStore")]
      ITransactionalState<StorageValue> value) =>
      this._value = value;

    [Transaction(TransactionOption.Join)]
    public Task Put(StorageValue value) =>
      _value.PerformUpdate(x => x.Value = value.Value);

    [Transaction(TransactionOption.Join)]
    public Task<StorageValue> Get() => 
      _value.PerformRead(x => x);

    [Transaction(TransactionOption.Join)]
    public Task Delete() => 
      _value.PerformUpdate(x => x.Value = null);

    public Task SetExpiry(TimeSpan timespan) =>
      throw new NotSupportedException();
  }
}

