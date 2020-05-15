// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.7.0
// Entry file
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyEchoBot.Services;
using MyEchoBot.Bots;

namespace MyEchoBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        // Facilitate dependancy injection
        public void ConfigureServices(IServiceCollection services)
        {   
            // starts here...
            // Setting project to be web api project 

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            // app can opt in or out...
            // Takes care of credential provider
            // Botframework adapter translates to what a bot can understand
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Configure state before we intialize the greeting bot
            ConfigureState(services);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            // How we connect the bot to the system...
            services.AddTransient<IBot, GreetinBot>();
        }

        public void ConfigureState(IServiceCollection services){
            // create the storage we'll be using for user and conversation state (Memory is great for testing purposes)
            services.AddSingleton<IStorage,MemoryStorage>();
            // create singletons for the user state
            services.AddSingleton<UserState>();
            // create the conversation state
            services.AddSingleton<ConversationState>();
            // create an instance of the state service
            services.AddSingleton<BotStateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
