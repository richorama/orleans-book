using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{

  public class RobotState
  {
    public Queue<string> Instructions { get; set; }
  }

  public class RobotGrain : Grain<RobotState>, IRobotGrain
  {
    ILogger<RobotGrain> logger;

    public RobotGrain(ILogger<RobotGrain> logger)
    {
      this.logger = logger;
      if (null == this.State)
      {
        this.State = new RobotState {
          Instructions = new Queue<string>()
        };
      }
    }
    
    public async Task AddInstruction(string instruction)
    {
      var key = this.GetPrimaryKeyString();
      this.logger.LogWarning(
        $"{key} adding '{instruction}'");
      this.State.Instructions.Enqueue(instruction);
      await this.WriteStateAsync();
    }

    public Task<int> GetInstructionCount()
    {
      return Task.FromResult(this.State.Instructions.Count);
    }

    public Task<string> GetNextInstruction()
    {
      if (this.State.Instructions.Count == 0)
      {
        return Task.FromResult<string>(null);
      }
      var instruction = this.State.Instructions.Dequeue();
      var key = this.GetPrimaryKeyString();      
      this.logger.LogWarning(
        $"{key} returning '{instruction}'");
      return Task.FromResult(instruction);
    }
  }
}
