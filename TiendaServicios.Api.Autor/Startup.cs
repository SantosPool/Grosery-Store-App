using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TiendaServicios.Api.Autor.Aplicacion;
using TiendaServicios.Api.Autor.ManejadorRabbit;
using TiendaServicios.Api.Autor.Persistencia;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.EventoQueue;
using TiendaServicios.RabbitMQ.Bus.Implement;

namespace TiendaServicios.Api.Autor
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
            //se inicaliza fluent validation dentro de nuevo, sin embargo busca todas las clases que hereden de abtrastac validator
            services.AddControllers().AddFluentValidation(cfg=>cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());
            services.AddDbContext<ContextoAutor>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("ConexionDatabase"));
            });

            //se agrega media tr como servicio para aceptar las solicitudes
            services.AddMediatR(typeof(Nuevo.Manejador).Assembly);

            //para la parte de automapper nuget instalado para DTO, solo es necesario registrarlo una vez las siguientes veces las hace de forma automatica
            services.AddAutoMapper(typeof(Consulta.Manejador));

            //para manejo del manejador del evento Email de RabbitMQ
            services.AddTransient<IEventoManejador<EmailEventoQueue>, EmailEventoManejador>();

            //para manejo RabbitEvent Bus de RabbitMQ
            services.AddTransient<IRabbitEventBus, RabbitEventBus>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //registro de EventBus de RabbitMQ
            var eventBus = app.ApplicationServices.GetRequiredService<IRabbitEventBus>();
            eventBus.Suscribe<EmailEventoQueue, EmailEventoManejador>();
        }
    }
}
