using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Modelo;
using TiendaServicios.Api.CarritoCompra.Persistencia;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public DateTime? FechaCreacionSesion { get; set; }
            public List<string> ProductoLista { get; set; }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CarritoContexto context;
            public Manejador(CarritoContexto _context)
            {
                context = _context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion = new CarritoSesion
                {
                    FechaCreacion= request.FechaCreacionSesion
                };
                context.CarritoSesion.Add(carritoSesion);

                var value= await context.SaveChangesAsync();
                if (value == 0)
                    throw new Exception("Errores en la insercion del carrito de compras");

                int id= carritoSesion.CarritoSesionId;

                var carritoSesionDetalle = request.ProductoLista.Select(x=>
                    new CarritoSesionDetalle
                    {
                        FechaCreacion = DateTime.Now,
                        CarritoSesionId=id,
                        ProductoSeleccionado=x
                    }).ToList();

                context.CarritoSesionDetalle.AddRange(carritoSesionDetalle);

                value= await context.SaveChangesAsync();

                if (value > 0)
                    return Unit.Value;

                throw new Exception("Errores en la insercion en detalles en el carrito de compras");

            }
        }
    }
}
