namespace OrleansBook.GrainInterfaces
{
  public class ValueDelta<T>
  {
    public ValueDelta()
    {}

    public ValueDelta(T oldValue, T newValue)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public T OldValue { get; set; }
    public T NewValue { get; set; }
  }

}
