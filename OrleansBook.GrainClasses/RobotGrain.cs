using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class RobotGrain : Grain, IRobotGrain
  {
    private Queue<string> instructions = new Queue<string>();

    public Task AddInstruction(string instruction)
    {
      this.instructions.Enqueue(instruction);
      return Task.CompletedTask;
    }

    public Task<int> GetInstructionCount()
    {
      return Task.FromResult(this.instructions.Count);
    }

    public Task<string> GetNextInstruction()
    {
      if (this.instructions.Count == 0) return Task.FromResult<string>(null);
      return Task.FromResult(this.instructions.Dequeue());
    }
  }
}
