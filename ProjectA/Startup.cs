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
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using ProjectA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectA
{
    public class Startup
    {
        private const int WaitAndRetryCount = 4;
        private const int ConsecutiveErrorsAllowedBeforeBreaking = 3;
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

            services.AddHttpClient<IRequestStringService, RequestStringService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44383/api/ProjectB");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1))
            .AddPolicyHandler(GetRetryPolicy());

            //.AddPolicyHandler(GetCircuitBreakerPolicy())
            //.AddPolicyHandler(GetFallbackPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            Random jitterer = new Random();

            return HttpPolicyExtensions
                 .HandleTransientHttpError()
                 .OrResult(res => !res.IsSuccessStatusCode) 
                 .Or<TimeoutRejectedException>() 
                 .WaitAndRetryAsync(
                     WaitAndRetryCount,
                     retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                           + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                     onRetry: (response, span, retryCount, context) =>
                     {
                         Console.WriteLine($"Retry count: {retryCount}");
                         Console.WriteLine($"Response: {response}");
                         Console.WriteLine($"Span: {span}", span);
                         Console.WriteLine($"Context: {context}");
                     });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(ConsecutiveErrorsAllowedBeforeBreaking, TimeSpan.FromSeconds(30)); 

            // open

            // closed 

            // half-open
        }

        private static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).Or<HttpRequestException>()
                .FallbackAsync(new HttpResponseMessage()
                {
                    Content = new StringContent("The temperature was 21 degrees at 21:00 according to the cache")
                });
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
