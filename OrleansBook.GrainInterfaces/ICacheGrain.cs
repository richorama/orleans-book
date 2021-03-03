using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{
  public interface ICacheGrain<T>: IGrainWithStringKey
  {
    Task Put(T value);

    Task<T> Get();

    Task Delete();
  }
}

