using System;
using Orleans;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi.Controllers
{
  [ApiController]
  public class RobotController : ControllerBase
  {
    private readonly IClusterClient _client;

    public RobotController(IClusterClient client)
    {
      _client = client;
    }

    [HttpGet]
    [Route("robot/{name}/instruction")]
    public Task<string> Get(string name)
    {
      var grain = this._client.GetGrain<IRobotGrain>(name);
      return grain.GetNextInstruction();
    }

    [HttpPost]
    [Route("robot/{name}/instruction")]
    public async Task<IActionResult> Post(string name, StorageValue value)
    {
      var grain = this._client.GetGrain<IRobotGrain>(name);
      await grain.AddInstruction(value.Value);
      return Ok();
    }

  }
}
