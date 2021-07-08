using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans.TestingHost;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.Tests
{
  [TestClass]
  public class UnitTest1
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
    public async Task TestAddOne()
    {
      var hello = cluster.GrainFactory.GetGrain<IExampleGrain>("test");
      Assert.AreEqual(1, await hello.AddOne());
      Assert.AreEqual(2, await hello.AddOne());
    }
  }
}
