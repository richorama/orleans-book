using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Orleans.Streams;
using OrleansBook.GrainInterfaces;

namespace OrleansBook.WebApi
{

  class StreamSubscriber : IAsyncObserver<ValueDelta<StorageValue>>
  {
    public Task OnCompletedAsync()
    {
      Console.WriteLine("Completed");
      return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
      Console.WriteLine("Exception");
      Console.WriteLine(ex.ToString());
      return Task.CompletedTask;
    }

    public Task OnNextAsync(ValueDelta<StorageValue> item, StreamSequenceToken token = null)
    {
      Console.WriteLine($"{item.OldValue?.Value ?? "null"} => {item.NewValue?.Value}");
      return Task.CompletedTask;
    }
  }

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    async Task<IClusterClient> ConnectToOrleans()
    {
      var client = new ClientBuilder()
        .UseLocalhostClustering()
        .AddSimpleMessageStreamProvider("SMSProvider")
        .Build();

      await client.Connect();
      var streamProvider  = client.GetStreamProvider("SMSProvider");
      var stream = streamProvider.GetStream<ValueDelta<StorageValue>>(Guid.Empty, "Delta");
      await stream.SubscribeAsync(new StreamSubscriber());
      return client;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      var client = ConnectToOrleans().Result;
      services.AddSingleton<IClusterClient>(client);

      

      services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
