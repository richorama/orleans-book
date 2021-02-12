using System.Threading.Tasks;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class CacheGrain<T> : Grain<T>, ICacheGrain<T>
  {
    public async Task Put(T value)
    {
      this.State = value;
      await this.WriteStateAsync();
    }
    public override Task OnActivateAsync()
    {
      return base.OnActivateAsync();
    }
    public Task<T> Get()
    {
      return Task.FromResult(this.State);
    }

    public async Task Delete()
    {
      await this.ClearStateAsync();
      this.DeactivateOnIdle();
    }
  }
}
