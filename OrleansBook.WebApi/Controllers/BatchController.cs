using System;
using Orleans;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace OrleansBook.WebApi.Controllers
{
  [ApiController]
  public class BatchController : ControllerBase
  {
    private readonly IClusterClient _client;

    public BatchController(IClusterClient client)
    {
      _client = client;
    }

    [HttpGet]
    [Route("batch/{key}")]
    public async Task<StorageValue> Get(string key)
    {
      var grain = this._client.GetGrain<ICacheGrain<StorageValue>>("x", "StorageValueGrains");
      var result = await grain.Get();

      if (result != null) return result;
      return new StorageValue { Value = null };
    }

    [HttpPost]
    [Route("batch")]
    public async Task<IActionResult> Post(IDictionary<string,string> values)
    {
      var grain = this._client.GetGrain<IBatchGrain>(0);

      var input = values.Select(keyValue => new WithKey<StorageValue>{
        Key = keyValue.Key,
        Value = new StorageValue{
          Value= keyValue.Value
        }
      }).ToArray();
      await grain.Put(input);

      return Ok();
    }

    
  }
}
