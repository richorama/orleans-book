using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  [ImplicitStreamSubscription("Delta")]
  public class SubscriberGrain : Grain, ISubscriberGrain,  IAsyncObserver<Delta<StorageValue>>
  {
    public override async Task OnActivateAsync()
    {
      var key = this.GetPrimaryKey();
      
      await GetStreamProvider("SMSProvider")
        .GetStream<Delta<StorageValue>>(key, "Delta")
        .SubscribeAsync(this);

      await base.OnActivateAsync();
    }

    public Task OnNextAsync(Delta<StorageValue> item, StreamSequenceToken token = null)
    {
      var oldValue = item.OldValue.Value ?? "null";
      var newValue = item.NewValue.Value ?? "null";
      var msg = $"{item.Key} : {oldValue} => {newValue}";
      Console.WriteLine(msg);
      return Task.CompletedTask;
    }
    public Task OnCompletedAsync() => Task.CompletedTask;
    public Task OnErrorAsync(System.Exception ex) => Task.CompletedTask;
  }
}