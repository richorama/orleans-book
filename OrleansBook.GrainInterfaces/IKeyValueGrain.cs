using System;
using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{
  public interface IKeyValueGrain: IGrainWithStringKey
  {
    
    [Transaction(TransactionOption.CreateOrJoin)]
    Task Put(string value);

    [Transaction(TransactionOption.CreateOrJoin)]
    Task<string> Get();

  }
}

