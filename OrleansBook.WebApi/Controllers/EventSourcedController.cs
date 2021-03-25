﻿using System;
using Orleans;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrleansBook.GrainInterfaces;
using System.Linq;

namespace OrleansBook.WebApi.Controllers
{
  [ApiController]
  public class EventSourcedController : ControllerBase
  {
    private readonly IClusterClient _client;

    public EventSourcedController(IClusterClient client) => 
      _client = client;

    [HttpPost]
    [Route("event/{key}")]
    public async Task<IActionResult> Post(string key, StorageValue value)
    {
      var grain = this._client.GetGrain<IEventSourcedGrain>(key);
      await grain.Next(new PerformUpdate(value.Value));
      return Ok();
    }
    
    [HttpGet]
    [Route("event/{key}")]
    public async Task<string> Get(string key)
    {
      var grain = this._client.GetGrain<IEventSourcedGrain>(key);
      var result = await grain.Get();
      return $"{result.Item1}, version {result.Item2}";
    }

    [HttpGet]
    [Route("event/{key}/history/{count:int}")]
    public async Task<string[]> GetHistory(string key, int count)
    {
      var grain = this._client.GetGrain<IEventSourcedGrain>(key);
      var result = await grain.GetHistory(count);
      return result.Select(x => x.ToString()).ToArray();
    }

  }
}
