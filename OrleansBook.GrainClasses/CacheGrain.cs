using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class CacheGrain<T> : Grain<T>, ICacheGrain<T>
  {
    private ILogger<ExampleGrain> _logger;

    public CacheGrain(ILogger<ExampleGrain> logger)
    {
      _logger = logger;
    }

    public override Task OnActivateAsync()
    {
      this._logger.LogInformation("Activating {0}", this.GetPrimaryKeyString());
      return base.OnActivateAsync();
    }

    public override Task OnDeactivateAsync()
    {
      this._logger.LogInformation("Deactivating {0}", this.GetPrimaryKeyString());
      return base.OnDeactivateAsync();
    }

    public async Task Put(T value)
    {
      this._logger.LogInformation("Write {0}", this.GetPrimaryKeyString());
      this.State = value;
      await this.WriteStateAsync();
    }

    public Task<T> Get()
    {
      this._logger.LogInformation("Read {0}", this.GetPrimaryKeyString());
      return Task.FromResult(this.State);
    }

    public async Task Delete()
    {
      this._logger.LogInformation("Delete {0}", this.GetPrimaryKeyString());
      await this.ClearStateAsync();
      this.DeactivateOnIdle();
    }
  }
}
