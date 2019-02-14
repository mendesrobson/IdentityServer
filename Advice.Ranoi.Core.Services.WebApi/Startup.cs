using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Services.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Este método é chamado pelo tempo de execução. Use este método para adicionar serviços ao contêiner.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Adicione quaisquer módulos ou registros do Autofac.
            // Isso é chamado DEPOIS ConfigureServices, então as coisas que você
            // registre-se aqui OVERRIDE as coisas registradas no ConfigureServices.
            //
            // Você deve ter a chamada para AddAutofac no Program.Main
            // método ou isso não será chamado.
            builder.RegisterModule<MainModule>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
