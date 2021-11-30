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
    int instructionsEnqueued = 0;
    int instructionsDequeued = 0;
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
      this.instructionsEnqueued += 1;
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
      this.instructionsDequeued += 1;
      await this.state.WriteStateAsync();
      return instruction;
    }

    public override Task OnActivateAsync()
    {
      var oneMinute = TimeSpan.FromMinutes(1);
      this.RegisterTimer(this.ResetStats, null, oneMinute, oneMinute);
      return base.OnActivateAsync();
    }

    Task ResetStats(object _)
    {
      var key = this.GetPrimaryKeyString();

      Console.WriteLine($"{key} enqueued: {this.instructionsEnqueued}");
      Console.WriteLine($"{key} dequeued: {this.instructionsDequeued}");
      Console.WriteLine($"{key} queued: {this.state.State.Instructions.Count}");

      this.instructionsEnqueued = 0;
      this.instructionsDequeued = 0;

      return Task.CompletedTask;
    }
  }
}