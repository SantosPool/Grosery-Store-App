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
    public class Consulta
    {
        public class Ejecuta: IRequest<List<LibreriaMaterialDto>> { }

        public class Manejador : IRequestHandler<Ejecuta, List<LibreriaMaterialDto>>
        {
            private readonly ContextoLibreria context;
            private readonly IMapper mapper;

            public Manejador(ContextoLibreria _context, IMapper _mapper)
            {
                context = _context;
                mapper = _mapper;
            }
            public async Task<List<LibreriaMaterialDto>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libros= await context.LibreriaMaterial.ToListAsync();
                var librosDto = mapper.Map<List<LibreriaMaterial>, List<LibreriaMaterialDto>>(libros);

                return librosDto;

            }
        }
    }
}
