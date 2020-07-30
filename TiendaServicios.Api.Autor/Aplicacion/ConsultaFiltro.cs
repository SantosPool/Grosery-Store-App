using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Modelo;
using TiendaServicios.Api.Autor.Persistencia;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class ConsultaFiltro
    {
        public class AutorUnico: IRequest<AutorDto>
        {
            public string AutorGuid { get; set; }
        }
        public class Manejador : IRequestHandler<AutorUnico, AutorDto>
        {
            private readonly ContextoAutor context;
            private readonly IMapper mapper;
            public Manejador(ContextoAutor _context, IMapper _mapper)
            {
                context = _context;
                mapper = _mapper;
            }
            public async Task<AutorDto> Handle(AutorUnico request, CancellationToken cancellationToken)
            {
                var autor = await context.AutorLibro.Where(x => x.AutorLibroGuid == request.AutorGuid).FirstOrDefaultAsync();
                
                if (autor is null)
                    throw new Exception("No se encontro el autor con el id correspondiete");

                var autorDto = mapper.Map<AutorLibro, AutorDto>(autor);
       

                return autorDto;           
            }
        }
    }
}
