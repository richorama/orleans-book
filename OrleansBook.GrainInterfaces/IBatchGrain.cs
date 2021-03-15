using System;
using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{
  public class WithKey<T>
  {
    public string Key { get; set; }
    public T Value { get; set; }
  }

  public interface IBatchGrain : IGrainWithIntegerKey
  {
    [Transaction(TransactionOption.Create)]
    Task Put(WithKey<StorageValue>[] values);
  }
}

