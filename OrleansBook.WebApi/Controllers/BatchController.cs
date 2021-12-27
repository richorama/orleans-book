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

    public BatchController(IClusterClient client) => 
      _client = client;

    [HttpPost]
    [Route("batch")]
    public async Task<IActionResult> Post(IDictionary<string,string> values)
    {
      var grain = this._client.GetGrain<IBatchGrain>(0);

      var input = values
        .Select(keyValue => (keyValue.Key, keyValue.Value))
        .ToArray();

      await grain.AddInstructions(input);

      return Ok();
    }
  }
}
