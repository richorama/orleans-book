using System;
using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{
  public interface IUpdate
  {

  }

  public class PerformUpdate : IUpdate
  {
    public string Value { get; }
    public PerformUpdate() { }
    public PerformUpdate(string value) =>
      this.Value = value;

    public override string ToString()
    {
      return $"Update to {this.Value}";
    }
  }

  public class EventSourcedState
  {
    public string Value { get; set; }

    public void Apply(PerformUpdate @event) =>
      this.Value = @event.Value;
  }

  public interface IEventSourcedGrain : IGrainWithStringKey
  {
    Task Next(IUpdate value);
    Task<(string, int)> Get();
    Task<IUpdate[]> GetHistory(int count);
  }

}

