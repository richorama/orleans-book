using System;
using System.Threading.Tasks;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class CacheGrain<T> : Grain<T>, ICacheGrain<T>, IRemindable
  {

    int totalReads = 0;
    int totalWrites = 0;
    int readsInLastMin = 0;
    int writesInLastMin = 0;

    public override Task OnActivateAsync()
    {
      var oneMinute = TimeSpan.FromMinutes(1);
      this.RegisterTimer(this.ResetStats, null, oneMinute, oneMinute);

      return base.OnActivateAsync();
    }

    Task ResetStats(object _)
    {
      this.totalReads += this.readsInLastMin;
      this.totalWrites += this.writesInLastMin;

      var key = this.GetPrimaryKeyString();

      Console.WriteLine($"{key} total reads: {this.totalReads}");
      Console.WriteLine($"{key} total writes: {this.totalWrites}");
      Console.WriteLine($"{key} reads in last min: {this.readsInLastMin}");
      Console.WriteLine($"{key} writes in last min: {this.writesInLastMin}");

      this.readsInLastMin = 0;
      this.writesInLastMin = 0;

      return Task.CompletedTask;
    }

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
      this.writesInLastMin += 1;
      var oldValue = this.State;
      this.State = value;
      await this.WriteStateAsync();
      await this.Publish(oldValue, value);
    }

    public Task<T> Get()
    {
      this.readsInLastMin += 1;
      return Task.FromResult(this.State);
    }

    public async Task Delete()
    {
      var key = this.GetPrimaryKeyString();

      foreach (var reminder in await this.GetReminders())
      {
        await this.UnregisterReminder(reminder);
      }

      await this.Publish(this.State, default(T));
      await this.ClearStateAsync();
      this.DeactivateOnIdle();
    }

    public Task SetExpiry(TimeSpan timespan)
    {
      var oneHour = TimeSpan.FromHours(1);
      return this.RegisterOrUpdateReminder("expire", timespan, oneHour);
    }

    public Task ReceiveReminder(string reminderName, Orleans.Runtime.TickStatus status)
    {
      if (reminderName == "expire") return this.Delete();
      return Task.CompletedTask;
    }
  }
}
