using AutoMapper;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using Xunit;

namespace TiendaServicios.Api.libro.Tests
{
    public class LibrosServiceTest
    {
        private IEnumerable<LibreriaMaterial> ObtenerDataPrueba()
        {
            A.Configure<LibreriaMaterial>()
                .Fill(x => x.Titulo).AsArticleTitle()
                .Fill(x => x.LibreriaMaterialId, () => { return Guid.NewGuid(); });

            var lista = A.ListOf<LibreriaMaterial>(30);
            lista[0].LibreriaMaterialId = Guid.Empty;


            return lista;
        }

        private Mock<ContextoLibreria> CrearContexto()
        {
            var dataPrueba = ObtenerDataPrueba().AsQueryable();

            var dbSet = new Mock<DbSet<LibreriaMaterial>>();
            //se indica que la clase sera de tipo entidad, propiedades que debe tener toda clase de entity Framework
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider).Returns(dataPrueba.Provider);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Expression).Returns(dataPrueba.Expression);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.ElementType).Returns(dataPrueba.ElementType);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.GetEnumerator()).Returns(dataPrueba.GetEnumerator());

            //configuracion para entity framework local
            dbSet.As<IAsyncEnumerable<LibreriaMaterial>>().Setup(x=>x.GetAsyncEnumerator(new System.Threading.CancellationToken()))
                .Returns(new AsyncEnumerator<LibreriaMaterial>(dataPrueba.GetEnumerator()));

            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider)
                .Returns(new AsyncQueryProvider<LibreriaMaterial>(dataPrueba.Provider));

            var contexto = new Mock<ContextoLibreria>();
            contexto.Setup(x => x.LibreriaMaterial).Returns(dbSet.Object);
            return contexto;
        }

        [Fact]
        public async void GetLibroPorId()
        {
            //System.Diagnostics.Debugger.Launch();
            var mockContexto = CrearContexto();

            var mapConfig = new MapperConfiguration(cfg =>
             {
                 cfg.AddProfile(new MappingTest());
             });

            var mapper = mapConfig.CreateMapper();

            var request = new ConsultaFiltro.LibroUnico();
            request.LibroId =Guid.Empty;

            var manejador = new ConsultaFiltro.Manejador(mockContexto.Object, mapper);

            var libro= await manejador.Handle(request, new System.Threading.CancellationToken());

            Assert.NotNull(libro);
            Assert.True(libro.LibreriaMaterialId == Guid.Empty);
        }

        [Fact]
        public async void GetLibros()
        {
            //System.Diagnostics.Debugger.Launch();
            //que metodo dentro de mi ms se esta encargando de realizar la consulta de libros de BD

            //1.- Emular instancia de EntityFrameworkCore- ContextoLibreria
            //para emular las acciones y eventos de un objeto en un ambiente de unit test utilizamos objetos de tipo Mock
            //un mock es la representacion de un objeto que puede actuar como un objeto real, pero solo funciona en test
            var mockContexto = CrearContexto();

            //2.- Emular al IMapper
            var mapconfig = new MapperConfiguration(cfg=> {
                cfg.AddProfile(new MappingTest());
            });
            var mapper = mapconfig.CreateMapper();
            //3.- instancias a las clase manejador y pasarle como parametros los mocks que he creado
            Consulta.Manejador manejador = new Consulta.Manejador(mockContexto.Object, mapper);

            Consulta.Ejecuta request = new Consulta.Ejecuta();

            var lista= await manejador.Handle(request, new System.Threading.CancellationToken());
            Assert.True(lista.Any());
        }

        [Fact]
        public async void GuardarLibro()
        {
            System.Diagnostics.Debugger.Launch();

            var options = new DbContextOptionsBuilder<ContextoLibreria>()
                .UseInMemoryDatabase(databaseName: "BaseDatosLibro")
                .Options;

            var contexto = new ContextoLibreria(options);

            var request = new Nuevo.Ejecuta();
            request.Titulo = "Libro Microservices";
            request.AutorLibro = Guid.Empty;
            request.FechaPublicacion = DateTime.Now;

            var manejador = new Nuevo.Manejador(contexto);
            var libro= await manejador.Handle(request, new System.Threading.CancellationToken());
            Assert.True(libro != null);
        }

    }
}
