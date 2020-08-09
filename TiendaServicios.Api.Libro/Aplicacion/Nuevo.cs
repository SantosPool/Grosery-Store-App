using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.EventoQueue;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta: IRequest
        {
            public string Titulo { get; set; }
            public DateTime? FechaPublicacion { get; set;}
            public Guid? AutorLibro { get; set; }
        }
        public class EjecutaValidacion: AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
                RuleFor(x => x.AutorLibro).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoLibreria context;
            private readonly IRabbitEventBus eventBus;
            public Manejador(ContextoLibreria _context, IRabbitEventBus _eventBus)
            {
                context = _context;
                eventBus = _eventBus;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libro = new LibreriaMaterial
                {
                    Titulo = request.Titulo,
                    FechaPublicacion = request.FechaPublicacion,
                    AutorLibro = request.AutorLibro,
                    LibreriaMaterialId = Guid.NewGuid()
                };

                context.LibreriaMaterial.Add(libro);

                var resultado = await context.SaveChangesAsync();

                eventBus.Publish(new EmailEventoQueue("santos.pool.nahuat@gmail.com", request.Titulo, "Este Contenido es un ejemplo"));
                
                if (resultado > 0)
                    return Unit.Value;

                throw new Exception("No se pudo Guardar el libro");
            }
        }
    }
}
