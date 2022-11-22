using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GrpcService.Services;
namespace GrpcService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(opt =>
            {
                opt.MaxReceiveMessageSize = 1 * 1024 * 1024;
                opt.MaxSendMessageSize = 100 * 1024 * 1024;
            }).AddServiceOptions<TelemetryService>(opt =>
            {
                opt.Interceptors.Add<ExampleServerInterceptor>();
            });
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
                endpoints.MapGrpcService<MessagingService>();
                endpoints.MapGrpcService<TelemetryService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Client requires Grpc.Net.Client + option in Protobuf was made to GENERATE Client and Service abstract classes!!!");
                });
            });
        }
    }
}
