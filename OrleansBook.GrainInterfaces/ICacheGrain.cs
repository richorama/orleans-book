using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{

  public class StorageValue
  {
    public string Value { get; set; }
  }


  public interface ICacheGrain<T>: IGrainWithStringKey
  {
    Task Put(T value);

    Task<T> Get();

    Task Delete();
  }
}
