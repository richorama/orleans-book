using System;
using System.Threading.Tasks;
using Orleans.Streams;

namespace OrleansBook.GrainInterfaces
{
  public class StreamSubscriber : IAsyncObserver<InstructionMessage>
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
      InstructionMessage instruction,
      StreamSequenceToken token = null)
    {
      var msg = $"{instruction.Robot} starting \"{instruction.Instruction}\"";
      Console.WriteLine(msg);
      return Task.CompletedTask;
    }
  }
}