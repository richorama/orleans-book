using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace OrleansBook.GrainInterfaces
{
  public interface IUpdate
  {

  }

  public class EnqueueInstruction : IUpdate
  {
    public string Value { get; }
    public EnqueueInstruction() { }
    public EnqueueInstruction(string value) =>
      this.Value = value;

    public override string ToString()
    {
      return $"Enqueue {this.Value}";
    }
  }

  public class DequeueInstruction : IUpdate
  {
    public string Value { get; set; }
    public DequeueInstruction() { }

    public override string ToString()
    {
      return $"Dequeue to {this.Value}";
    }
  }


  public class EventSourcedState
  {
    Queue<string> instructions = new Queue<string>();

    public int Count => this.instructions.Count;

    public void Apply(EnqueueInstruction @event) =>
      this.instructions.Enqueue(@event.Value);

    public void Apply(DequeueInstruction @event)
    {
      if (this.instructions.Count == 0) return;
      @event.Value = this.instructions.Dequeue();
    }
  }



}

