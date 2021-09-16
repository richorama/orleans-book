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
  class SiloBuilderConfigurator : ISiloConfigurator
  {
    public void Configure(ISiloBuilder hostBuilder)
    {
      hostBuilder.AddMemoryGrainStorage("robotStateStore");

      var mockState = new Mock<IPersistentState<RobotState>>();
      mockState.SetupGet(s => s.State).Returns(new RobotState
      {
        Instructions = new Queue<string>()
      });

      var mockMapper = new Mock<IAttributeToFactoryMapper<PersistentStateAttribute>>();
      mockMapper.Setup(
        o => 
          o.GetFactory(
              It.IsAny<ParameterInfo>(),
              It.IsAny<PersistentStateAttribute>()))
        .Returns(_ => mockState.Object);

      hostBuilder.ConfigureServices(services =>
      {
        services.AddSingleton(mockMapper.Object);
        services.AddSingleton<ILogger<RobotGrain>>(new Mock<ILogger<RobotGrain>>().Object);
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
