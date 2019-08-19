// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShadyBot.Bots;
using ShadyBot.Dialogs;
using ShadyBot.Services;

using Microsoft.BotBuilderSamples.Bots;

namespace Microsoft.BotBuilderSamples
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            ConfigureState(services);
            ConfigureDialogs(services);

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            // services.AddTransient<IBot, EchoBot>();
            // services.AddTransient<IBot, GreetingBot>();
            services.AddTransient<IBot, DialogBot<MainDialog>>();
        }

        public void ConfigureDialogs(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>();
        }

        public void ConfigureState(IServiceCollection services)
        {
            // Create storage for User and Conversation state. (MemStorage for development, port to CosmoDb or the like for prod use).
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User State.
            services.AddSingleton<UserState>();


            // Create the Conversation State.
            services.AddSingleton<ConversationState>();

            // Add instance of the state service.
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

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
