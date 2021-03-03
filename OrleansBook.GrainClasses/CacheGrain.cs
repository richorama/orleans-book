using System;
using System.Threading.Tasks;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class CacheGrain<T> : Grain<T>, ICacheGrain<T>
  {
    Task Publish(T oldValue, T newValue)
    {
      var message = new Delta<T>(
        this.GetPrimaryKeyString(),
        oldValue,
        newValue);

      return this
        .GetStreamProvider("SMSProvider")
        .GetStream<Delta<T>>(Guid.Empty, "Delta")
        .OnNextAsync(message);
    }

    public async Task Put(T value)
    {
      var oldValue = this.State;
      this.State = value;
      await this.WriteStateAsync();
      await this.Publish(oldValue, value);
    }

    public Task<T> Get()
    {
      return Task.FromResult(this.State);
    }

    public async Task Delete()
    {
      await this.ClearStateAsync();
      await this.Publish(this.State, default(T));
      this.DeactivateOnIdle();
    }
  }
}
