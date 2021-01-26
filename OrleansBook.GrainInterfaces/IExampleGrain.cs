using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{
  public interface IExampleGrain: IGrainWithStringKey
  {
    Task<int> AddOne();
  }
}
