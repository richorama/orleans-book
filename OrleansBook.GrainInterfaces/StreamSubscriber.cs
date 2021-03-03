using System;
using System.Threading.Tasks;
using Orleans.Streams;

namespace OrleansBook.GrainInterfaces
{
public class StreamSubscriber : IAsyncObserver<Delta<StorageValue>>
{
  public Task OnCompletedAsync()
  {
    Console.WriteLine("Completed");
    return Task.CompletedTask;
  }

  public Task OnErrorAsync(Exception ex)
  {
    Console.WriteLine("Exception");
    Console.WriteLine(ex.ToString());
    return Task.CompletedTask;
  }

  public Task OnNextAsync(
    Delta<StorageValue> item,
    StreamSequenceToken token = null)
  {
    var oldValue = item.OldValue.Value ?? "null";
    var newValue = item.NewValue.Value ?? "null";
    var msg = $"{item.Key} : {oldValue} => {newValue}";
    Console.WriteLine(msg);
    return Task.CompletedTask;
  }
}
}