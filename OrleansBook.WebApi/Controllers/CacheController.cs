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

    // curl http://localhost:5000/cache/123
    [HttpGet]
    [Route("cache/{key}")]
    public async Task<StorageValue> Get(string key)
    {
      var grain = this._client.GetGrain<ICacheGrain<StorageValue>>(key);
      var result = await grain.Get();

      if (result != null) return result;
      return new StorageValue { Value = null };
    }

    // curl --header "Content-Type: application/json" --request POST --data '{"Value": "Hello World"}' http://localhost:5000/cache/123
    [HttpPost]
    [Route("cache/{key}")]
    public async Task<IActionResult> Post(string key, StorageValue value)
    {
      var grain = this._client.GetGrain<ICacheGrain<StorageValue>>(key);
      await grain.Put(value);
      return Ok();
    }
  }
}
