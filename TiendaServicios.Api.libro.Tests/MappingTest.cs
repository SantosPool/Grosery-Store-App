using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;

namespace TiendaServicios.Api.libro.Tests
{
    public class MappingTest: Profile
    {
        public MappingTest()
        {
            CreateMap<LibreriaMaterial, LibreriaMaterialDto>();
        }
    }
}
