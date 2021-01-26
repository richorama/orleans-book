using System.Threading.Tasks;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class ExampleGrain : Grain, IExampleGrain
  {
    int _counter = 0;
    public Task<int> AddOne()
    {
      _counter += 1;
      return Task.FromResult(_counter);
    }
  }
}
