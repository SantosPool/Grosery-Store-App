using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Server.IIS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Modelo;
using TiendaServicios.Api.Autor.Persistencia;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public DateTime? FechaNacimiento { get; set; }
        }
        public class EjecutaValidacion: AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellido).NotEmpty();
                RuleFor(x => x.FechaNacimiento).NotEmpty();
            }

        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoAutor context;
            public Manejador(ContextoAutor _context)
            {
                context = _context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var autorLibro = new AutorLibro
                {
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    FechaNacimiento = request.FechaNacimiento ?? DateTime.UtcNow,
                    AutorLibroGuid= Convert.ToString( Guid.NewGuid()),
                };

                context.AutorLibro.Add(autorLibro);

                var resultado = await context.SaveChangesAsync();
                if (resultado > 0)
                    return Unit.Value;

                throw new Exception("No se pudo insertar el autor del libro");

            }
        }
    }
}
