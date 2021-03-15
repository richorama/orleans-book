using System;
using Orleans;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi.Controllers
{
  [ApiController]
  public class CacheController : ControllerBase
  {
    private readonly IClusterClient _client;

    public CacheController(IClusterClient client)
    {
      _client = client;
    }

    [HttpGet]
    [Route("cache/{key}")]
    public async Task<StorageValue> Get(string key)
    {
      var grain = this._client.GetGrain<ICacheGrain<StorageValue>>(key);
      var result = await grain.Get();

      if (result != null) return result;
      return new StorageValue { Value = null };
    }

    [HttpPost]
    [Route("cache/{key}")]
    public async Task<IActionResult> Post(string key, StorageValue value)
    {
      var grain = this._client.GetGrain<ICacheGrain<StorageValue>>(key);
      await grain.Put(value);
      return Ok();
    }

    [HttpPost]
    [Route("cache/{key}/expiry/{expiry:int}")]
    public async Task<IActionResult> Expire(string key, int expiry)
    {
      var grain = this._client.GetGrain<ICacheGrain<StorageValue>>(key);
      await grain.SetExpiry(TimeSpan.FromSeconds(expiry));
      return Ok();
    }
  }
}
