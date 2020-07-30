using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Autor.Modelo;

namespace TiendaServicios.Api.Autor.Aplicacion
{
    public class MappingProfile: Profile
    {

        public MappingProfile()
        {
            //necesario el creado de estos mapeos para el llamado en el API
            CreateMap<AutorLibro, AutorDto>();
        }
    }
}
