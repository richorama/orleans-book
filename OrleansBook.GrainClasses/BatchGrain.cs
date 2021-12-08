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
    public Task Put((string,string)[] values)
    {
      var tasks = values.Select(keyValue =>
        this.GrainFactory
          .GetGrain<IRobotGrain>(keyValue.Item1)
          .AddInstruction(keyValue.Item2)
      );

      return Task.WhenAll(tasks);
    }
  }
}

