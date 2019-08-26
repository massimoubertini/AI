using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Primo_Progetto_Bot.Bots;

namespace Primo_Progetto_Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // questo metodo è chiamato dal runtime: aggiunge servizi al contenitore
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // crea il Bot Framework Adapter con la gestione degli errori abilitata
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            // crea il bot come temporaneo, in questo caso il controller ASP prevede un IBot
            services.AddTransient<IBot, EchoBot>();
        }

        // questo metodo è chiamato dal runtime: configura la pipeline delle richieste HTTP
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}