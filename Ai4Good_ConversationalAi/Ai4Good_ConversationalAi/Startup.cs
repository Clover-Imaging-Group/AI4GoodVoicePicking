using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ai4Good_ConversationalAi.AzureSpeech;
using Ai4Good_ConversationalAi.Bots;
using Ai4Good_ConversationalAi.Common.Interfaces;
using Ai4Good_ConversationalAi.Hubs;
using Ai4Good_ConversationalAi.Implementation.Repositories;
using Ai4Good_ConversationalAi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ai4Good_ConversationalAi
{
    public class Startup
    {
        readonly string AllowOrigin = "AllowOrigin";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var speechConfigSection = Configuration.GetSection("AzureSpeechConfig");
            var speechConfig = new AzureSpeech.SpeechConfig()
            {
                AppName = speechConfigSection.GetSection("AppName").Value,
                AudioFormat = speechConfigSection.GetSection("AudioFormat").Value,
                AzureRegion = speechConfigSection.GetSection("AzureRegion").Value,
                APIKey = speechConfigSection.GetSection("APIKey").Value,
                Language = speechConfigSection.GetSection("Language").Value,
                VoiceName = speechConfigSection.GetSection("VoiceName").Value
            };

            services.AddTransient<ITranscriptRepository, TranscriptRepository>();
            services.AddTransient<IHubRepository, HubRepository>();
            services.AddSingleton<ISpeechService>(new SpeechService(speechConfig));
            services.AddSingleton(new LuisRecognizer(
                       new LuisApplication(
                           Configuration["DispatchAppId"],
                           Configuration["DispatchAPIKey"],
                           $"https://{ Configuration["DispatchAPIHostName"]}.api.cognitive.microsoft.com"
                       ),
                       new LuisPredictionOptions { IncludeAllIntents = true, IncludeInstanceData = true },
                       true)
               );
            var children = Configuration.GetSection("BotIntents").GetChildren();
            List<BotIntent> botIntents = children.Select(s => new BotIntent
            {
                Intent = s.GetSection("Intent").Value,
                Id = s.GetSection("Id").Value,
                Key = s.GetSection("Key").Value,
                HostName = s.GetSection("HostName").Value,
                IsLuis = Convert.ToBoolean(s.GetSection("IsLuis").Value),
            }).ToList();
            services.AddSingleton(botIntents);
            services.AddTransient<IBot, ConversationDispatcher>();

            services.AddRazorPages();
            services.AddCors(options => {
                options.AddPolicy(
                    AllowOrigin,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin();
                                });
            }
            );
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ConversationalHub>("/ConversationalHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
