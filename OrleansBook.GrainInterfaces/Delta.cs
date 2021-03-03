namespace OrleansBook.GrainInterfaces
{
  public class Delta<T>
  {
    public Delta()
    { }

    public Delta(string key, T oldValue, T newValue)
    {
      this.Key = key;
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public string Key { get; set; }
    public T OldValue { get; set; }
    public T NewValue { get; set; }
  }

}
