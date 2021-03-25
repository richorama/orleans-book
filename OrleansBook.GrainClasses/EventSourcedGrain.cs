using System;
using System.Threading.Tasks;
using OrleansBook.GrainInterfaces;
using Orleans.EventSourcing;
using System.Linq;

namespace OrleansBook.GrainClasses
{

  public class EventSourcedGrain : JournaledGrain<EventSourcedState, IUpdate>, IEventSourcedGrain
  {
    public async Task Next(IUpdate @event)
    {
      RaiseEvent(@event);
      await ConfirmEvents();
    }

    public Task<(string, int)> Get()
    {
      return Task.FromResult((this.State.Value, this.Version));
    }

    public async Task<IUpdate[]> GetHistory(int count)
    {
      var results = await RetrieveConfirmedEvents(Math.Max(0, this.Version - count), this.Version);
      return results.ToArray();
    }

  }
}

