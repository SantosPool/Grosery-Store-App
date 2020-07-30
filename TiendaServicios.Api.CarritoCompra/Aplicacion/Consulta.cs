using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.Persistencia;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;

namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class Consulta
    {
        public class Ejecuta: IRequest<CarritoDto>
        {
            public int CarritoSesionId { get; set; }
        }
        public class Manejador : IRequestHandler<Ejecuta, CarritoDto>
        {
            private readonly CarritoContexto context;
            private readonly ILibrosService librosService;
            public Manejador(CarritoContexto _context,ILibrosService _librosService)
            {
                context = _context;
                librosService = _librosService;
            }
            public async Task<CarritoDto> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var carritoSesion =await  context.CarritoSesion.FirstOrDefaultAsync(x => x.CarritoSesionId == request.CarritoSesionId);
                var carritoSesionDetalle = await context.CarritoSesionDetalle.Where(x => x.CarritoSesionId == request.CarritoSesionId).ToListAsync();
                var listaCarritoDto = new List<CarritoDetalleDto>();
                foreach(var libro in carritoSesionDetalle)
                {
                    var response= await  librosService.GetLibro(new Guid(libro.ProductoSeleccionado));
                    if (response.resultado)
                    {
                        var objetolibro = response.Libro;
                        var carritoDetalle = new CarritoDetalleDto
                        {
                            TituloLibro= objetolibro.Titulo,
                            FechaPublicacion= objetolibro.FechaPublicacion,
                            LibroId=objetolibro.LibreriaMaterialId
                        };
                        listaCarritoDto.Add(carritoDetalle);
                    }
                }
                var carritoSesionDto = new CarritoDto
                {
                    CarritoId=carritoSesion.CarritoSesionId,
                    FechaCreacionSesion= carritoSesion.FechaCreacion,
                    ListaProductos= listaCarritoDto
                };
                return carritoSesionDto;

            }
        }
    }
}
