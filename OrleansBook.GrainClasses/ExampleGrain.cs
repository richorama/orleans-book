using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class ExampleGrain : Grain, IExampleGrain
  {
    ILogger<ExampleGrain> _logger;

    public ExampleGrain(ILogger<ExampleGrain> logger)
    {
      _logger = logger;
    }
    
    int _counter = 0;
    public Task<int> AddOne()
    {
      _counter += 1;
      _logger.LogWarning("Grain {0} counter is now {1}", this.GetPrimaryKeyString(), _counter);
      return Task.FromResult(_counter);
    }
  }
}
