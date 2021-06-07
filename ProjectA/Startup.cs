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
            services.AddHttpClient("MyNamedClient", client => // Named Client : DELETE THIS COMMENT
            {
                client.BaseAddress = new Uri("https://localhost:44383/api/ProjectB");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3),
            }));

            services.AddHttpClient<IRequestStringService, RequestStringService>(client => // Typed Client : DELETE THIS COMMENT
            {
                client.BaseAddress = new Uri("https://localhost:44383/api/ProjectB");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Set life time to five minutes,
                                                             //.AddPolicyHandler(GetRetryPolicy());
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            Random jitterer = new Random();
            return HttpPolicyExtensions
                 .HandleTransientHttpError() // TODO: Find out what this does...
                 .OrResult(res => !res.IsSuccessStatusCode) // Retry if status code != 200
                 .Or<TimeoutRejectedException>() // Retry when TimeoutRejectedException are thrown
                 .WaitAndRetryAsync(
                      4,
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

        //private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        //{
        //    AsyncPolicy timeoutPolicy = Policy.TimeoutAsync(30);
        //    HttpResponseMessage httpResponse = timeoutPolicy
        //        .ExecuteAsync(
        //          async ct => await httpClient.GetAsync(endpoint, ct), // Execute a delegate which responds to a CancellationToken input parameter.
        //          CancellationToken.None // In this case, CancellationToken.None is passed into the execution, indicating you have no independent cancellation control you wish to add to the cancellation provided by TimeoutPolicy.  Your own indepdent CancellationToken can also be passed - see wiki for examples.
        //          );

        //}

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
