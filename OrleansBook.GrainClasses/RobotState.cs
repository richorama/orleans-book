using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.GrainClasses
{
  public class RobotState
  {
    public Queue<string> Instructions { get; set; } = new Queue<string>();
  }
  
}
