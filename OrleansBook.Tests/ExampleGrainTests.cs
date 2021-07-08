using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans.TestingHost;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.Tests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public async Task TestAddOne()
    {
      var cluster = new TestClusterBuilder().Build();
      cluster.Deploy();

      var hello = cluster.GrainFactory.GetGrain<IExampleGrain>("test");
      Assert.AreEqual(1, await hello.AddOne());
      Assert.AreEqual(2, await hello.AddOne());
      Assert.AreEqual(3, await hello.AddOne());

      cluster.StopAllSilos();


    }
  }
}
