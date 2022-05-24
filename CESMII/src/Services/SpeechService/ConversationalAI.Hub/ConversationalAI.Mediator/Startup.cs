using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConversationalAI.HubService.Repositories;
using ConversationalAI.Infrastructure.Interfaces.Repositories;
using ConversationalAI.Infrastructure.Interfaces.Services;
using ConversationalAI.SpeechService.Services;
using ConversationalAI.Mediator.Hubs;
using ConversationalAI.Mediator.Infrastructure;
using ConversationalAI.Mediator.Infrastructure.Jobs;
using ConversationalAI.Mediator.Infrastructure.Services;
using ConversationalAI.Mediator.Infrastructure.Services.Infrastructure;
using ConversationalAI.SpeechService.Models.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace ConversationalAI.Mediator
{
    public class Startup
    {
        private const string AllowOrigin = "AllowOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            
            
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            
            services.AddCors(options => {
                    options.AddPolicy(
                        AllowOrigin,
                        builder =>
                        {
                            builder.WithOrigins("https://localhost:63342", 
                                    "https://localhost:63343",
                                    "https://localhost:5001")
                                .AllowCredentials()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                }
            );
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
            });
            
            services.Configure<SpeechServiceSettings>(Configuration.GetSection("AzureSpeechConfig"));

            services.AddHttpClient<ISpeechService, SpeechService.Services.SpeechService>();
            services.AddSingleton<IHubRepository, HubRepository>();
            services.AddSingleton<IHubContextService, HubContextService>();
            
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var proactiveJobKey = new JobKey("proactiveMessagesJob");
                q.AddJob<ProactiveMessagesJob>(opts => opts.WithIdentity(proactiveJobKey));
                q.AddTrigger(opts => opts
                    .ForJob(proactiveJobKey)
                    .WithIdentity("proactiveMessagesJob-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(5)
                        .RepeatForever()));
            });
            
            services.AddQuartzHostedService(
                q => q.WaitForJobsToComplete = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(AllowOrigin);

            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ConversationalHub>("/ConversationalHub");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}