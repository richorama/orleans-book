using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class RobotGrain : Grain, IRobotGrain
  {
    ILogger<RobotGrain> logger;
    IPersistentState<RobotState> state;

    public RobotGrain(
      ILogger<RobotGrain> logger,
      [PersistentState("robotState", "robotStateStore")]
      IPersistentState<RobotState> state)
    {
      this.logger = logger;
      this.state = state;
    }

    Task Publish(string instruction)
    {
      var message = new InstructionMessage(
        instruction,
        this.GetPrimaryKeyString());

      return this
        .GetStreamProvider("SMSProvider")
        .GetStream<InstructionMessage>(Guid.Empty, "StartingInstruction")
        .OnNextAsync(message);
    }

    public async Task AddInstruction(string instruction)
    {
      var key = this.GetPrimaryKeyString();
      this.logger.LogWarning($"{key} adding '{instruction}'");
      this.state.State.Instructions.Enqueue(instruction);
      await this.state.WriteStateAsync();
    }

    public Task<int> GetInstructionCount()
    {
      return Task.FromResult(this.state.State.Instructions.Count);
    }

    public async Task<string> GetNextInstruction()
    {
      if (this.state.State.Instructions.Count == 0)
      {
        return null;
      }

      var instruction = this.state.State.Instructions.Dequeue();
      var key = this.GetPrimaryKeyString();

      this.logger.LogWarning(
        $"{key} returning '{instruction}'");

      await this.Publish(instruction);

      await this.state.WriteStateAsync();
      return instruction;
    }
  }
}