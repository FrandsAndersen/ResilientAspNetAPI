using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ProjectA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectA", Version = "v1" });
            });
            services.AddHttpClient("MyNamedClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44383/api/ProjectB");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3),
            }));
        }

        static Policy GetRetryPolicy()
        {
            var policy = Policy
                            .Handle<SocketException>()
                            .WaitAndRetry(new[]
                            {
                                TimeSpan.FromSeconds(1),
                                TimeSpan.FromSeconds(2),
                                TimeSpan.FromSeconds(3),
                            }, (exception, timeSpan, retryCount, context) =>
                            {
                                Console.WriteLine($"exception.message: {exception.Message}");
                                Console.WriteLine($"timeSpan: {timeSpan}");
                                Console.WriteLine($"retryCount: {retryCount}");
                                Console.WriteLine($"context: {context}");
                            });

            return policy;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectA v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
