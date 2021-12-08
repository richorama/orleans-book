using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Transactions.Abstractions;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class RobotGrain : Grain, IRobotGrain, IRemindable
  {
    int instructionsEnqueued = 0;
    int instructionsDequeued = 0;
    ILogger<RobotGrain> logger;
    ITransactionalState<RobotState> state;

    public RobotGrain(
      ILogger<RobotGrain> logger,
      [PersistentState("robotState", "robotStateStore")]
      ITransactionalState<RobotState> state)
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
      this.instructionsEnqueued += 1;
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
        await this.Publish(instruction);
        this.instructionsDequeued += 1;
      }
      return instruction;
    }

    public async override Task OnActivateAsync()
    {
      var oneMinute = TimeSpan.FromMinutes(1);
      this.RegisterTimer(this.ResetStats, null, oneMinute, oneMinute);

      var oneDay = TimeSpan.FromDays(1);
      await this.RegisterOrUpdateReminder("firmware", oneDay, oneDay);
      await this.AddInstruction("Update firmware");

      await base.OnActivateAsync();
    }

    public Task ReceiveReminder(string reminderName, Orleans.Runtime.TickStatus status)
    {
      if (reminderName == "firmware") return this.AddInstruction("Update firmware");
      return Task.CompletedTask;
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