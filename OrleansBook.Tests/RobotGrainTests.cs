using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.TestingHost;
using OrleansBook.GrainClasses;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.Tests
{

  class FakeState<T> : IPersistentState<T>
  {
    public T State {get;set;}

    public string Etag {get;set;}

    public bool RecordExists => this.State is not null;

    public Task ClearStateAsync()
    {
      this.State = default(T);
      return Task.CompletedTask;
    }

    public Task ReadStateAsync() =>
      Task.CompletedTask;

    public Task WriteStateAsync()
    {
      this.Etag = Guid.NewGuid().ToString();
      return Task.CompletedTask;
    }
  }

  class SiloBuilderConfigurator : ISiloConfigurator
{
  public void Configure(ISiloBuilder hostBuilder)
  {
    hostBuilder.AddMemoryGrainStorage("robotStateStore");

    var fakeState = new FakeState<RobotState>();
    
    hostBuilder.ConfigureServices(services =>
    {
      services.AddSingleton<IPersistentState<RobotState>>(fakeState);
      services.AddSingleton<ILogger<RobotGrain>>(
          new Mock<ILogger<RobotGrain>>().Object);
    });
  }
}


  [TestClass]
  public class RobotGrainTests
  {
    static TestCluster cluster;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
      cluster = new TestClusterBuilder()
        .AddSiloBuilderConfigurator<SiloBuilderConfigurator>()
        .Build();

      cluster.Deploy();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      cluster.StopAllSilos();
    }

    [TestMethod]
    public async Task TestInstructions()
    {
      var robot = cluster.GrainFactory.GetGrain<IRobotGrain>("test");

      await robot.AddInstruction("Do the dishes");
      await robot.AddInstruction("Take out the trash");
      await robot.AddInstruction("Do the laundry");

      Assert.AreEqual(3, await robot.GetInstructionCount());
      Assert.AreEqual(await robot.GetNextInstruction(), "Do the dishes");
      Assert.AreEqual(await robot.GetNextInstruction(), "Take out the trash");
      Assert.AreEqual(await robot.GetNextInstruction(), "Do the laundry");
      Assert.IsNull(await robot.GetNextInstruction());
      Assert.AreEqual(0, await robot.GetInstructionCount());
    }
  }
}
