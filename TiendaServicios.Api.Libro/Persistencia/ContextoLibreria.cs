using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Modelo;

namespace TiendaServicios.Api.Libro.Persistencia
{
    public class ContextoLibreria: DbContext
    {
        public ContextoLibreria() { }
        public ContextoLibreria(DbContextOptions<ContextoLibreria> _options): base(_options)
        {
        }
        //propiedad virtual que se puede sobreescribir
        public virtual DbSet<LibreriaMaterial> LibreriaMaterial { get; set; }
    }
}
