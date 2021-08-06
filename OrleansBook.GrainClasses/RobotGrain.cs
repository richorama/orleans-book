using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class RobotGrain : Grain, IRobotGrain
  {
    ILogger<RobotGrain> logger;

    public RobotGrain(ILogger<RobotGrain> logger)
    {
      this.logger = logger;
    }
    
    private Queue<string> instructions = new Queue<string>();

    public Task AddInstruction(string instruction)
    {
      var key = this.GetPrimaryKeyString();
      this.logger.LogWarning(
        $"{key} adding '{instruction}'");
      this.instructions.Enqueue(instruction);
      return Task.CompletedTask;
    }

    public Task<int> GetInstructionCount()
    {
      return Task.FromResult(this.instructions.Count);
    }

    public Task<string> GetNextInstruction()
    {
      if (this.instructions.Count == 0)
      {
        return Task.FromResult<string>(null);
      }
      var instruction = this.instructions.Dequeue();
      var key = this.GetPrimaryKeyString();      
      this.logger.LogWarning(
        $"{key} returning '{instruction}'");
      return Task.FromResult(instruction);
    }
  }
}
