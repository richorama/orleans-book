using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class RobotGrain : Grain, IRobotGrain
  {
    ILogger<RobotGrain> logger;
    ITransactionalState<RobotState> state;

    public RobotGrain(
      ILogger<RobotGrain> logger,
      [TransactionalState("robotState", "robotStateStore")]
      ITransactionalState<RobotState> state)
    {
      this.logger = logger;
      this.state = state;
    }

    public async Task AddInstruction(string instruction)
    {
      var key = this.GetPrimaryKeyString();
      this.logger.LogWarning($"{key} adding '{instruction}'");
      await this.state.PerformUpdate(state => state.Instructions.Enqueue(instruction));
    }

    public async Task<int> GetInstructionCount()
    {
      return await this.state.PerformRead(state => state.Instructions.Count);
    }

    public async Task<string> GetNextInstruction()
    {
      var key = this.GetPrimaryKeyString();
      string instruction = null;
      await this.state.PerformUpdate(state =>
      {
        if (state.Instructions.Count == 0) return;
        instruction = state.Instructions.Dequeue();
      });

      if (null != instruction)
      {
        this.logger.LogWarning(
          $"{key} returning '{instruction}'");
      }
      return instruction;
    }
  }
}