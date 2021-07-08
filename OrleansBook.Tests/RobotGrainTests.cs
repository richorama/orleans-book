using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans.TestingHost;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.Tests
{
  [TestClass]
  public class RobotGrainTests
  {
    static TestCluster cluster;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
      cluster = new TestClusterBuilder().Build();
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
