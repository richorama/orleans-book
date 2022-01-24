using System;
using System.Threading.Tasks;
using OrleansBook.GrainInterfaces;
using Orleans.EventSourcing;
using System.Linq;
using Orleans.Providers;

namespace OrleansBook.GrainClasses
{


  [StorageProvider(ProviderName = "robotStateStore")]
  public class EventSourcedGrain : JournaledGrain<EventSourcedState, IUpdate>, IRobotGrain
  {
    public async Task AddInstruction(string instruction)
    {
      RaiseEvent(new EnqueueInstruction(instruction));
      await ConfirmEvents();
    }

    public async Task<string> GetNextInstruction()
    {
      if (this.State.Count == 0) return null;
      
      var instruction = new DequeueInstruction();
      RaiseEvent(instruction);
      await ConfirmEvents();
      return instruction.Value;
    }

    public Task<int> GetInstructionCount()
    {
      return Task.FromResult(this.State.Count);
    }
  }
}

