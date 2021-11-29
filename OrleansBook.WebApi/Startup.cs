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

            await client
              .GetStreamProvider("SMSProvider")
              .GetStream<InstructionMessage>(Guid.Empty, "StartingInstruction")
              .SubscribeAsync(new StreamSubscriber());

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
