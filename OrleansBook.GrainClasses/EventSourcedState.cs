using System;
using System.Collections.Generic;

namespace OrleansBook.GrainClasses
{
  public class EventSourcedState
  {
    Queue<string> instructions = new Queue<string>();

    public int Count => this.instructions.Count;

    public void Apply(EnqueueEvent @event) =>
      this.instructions.Enqueue(@event.Value);

    public void Apply(DequeueEvent @event)
    {
      if (this.instructions.Count == 0) return;
      @event.Value = this.instructions.Dequeue();
    }
  }
}