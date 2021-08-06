using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans.Hosting;
using Orleans.TestingHost;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.Tests
{
  class SiloBuilderConfigurator : ISiloConfigurator
  {
      public void Configure(ISiloBuilder hostBuilder)
      {
        hostBuilder.AddMemoryGrainStorage("robotStateStore");
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
      Assert.IsNotNull(robot);
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
