using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class ConsultaFiltro
    {
        public class LibroUnico: IRequest<LibreriaMaterialDto>
        {
            public Guid? LibroId { get; set; }
        }
        public class Manejador : IRequestHandler<LibroUnico, LibreriaMaterialDto>
        {
            private readonly ContextoLibreria context;
            private readonly IMapper mapper;
            public Manejador(ContextoLibreria _context, IMapper _mapper)
            {
                context = _context;
                mapper = _mapper;
            }
            public async Task<LibreriaMaterialDto> Handle(LibroUnico request, CancellationToken cancellationToken)
            {
                var libro = await context.LibreriaMaterial.Where(x => x.LibreriaMaterialId == request.LibroId).FirstOrDefaultAsync();

                if (libro is null)
                    throw new Exception("no se encontro el libro con id indicado");

                var libroDto = mapper.Map<LibreriaMaterial, LibreriaMaterialDto>(libro);

                return libroDto;
            }
        }

    }
}
