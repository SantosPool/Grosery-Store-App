using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TiendaServicios.Api.Gateway.InterfaceRemote;
using TiendaServicios.Api.Gateway.LibroRemote;

namespace TiendaServicios.Api.Gateway.MessageHandler
{
    public class LibroHandler: DelegatingHandler
    {
        private readonly ILogger<LibroHandler> logger;
        private readonly IAutorRemote autorRemote;
        public LibroHandler(ILogger<LibroHandler> _logger, IAutorRemote _autorRemote)
        {
            logger = _logger;
            autorRemote = _autorRemote;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
        {
            var tiempo = Stopwatch.StartNew();
            logger.LogInformation("Inicia Request");

            var response =await  base.SendAsync(request, cancellationToken);
            logger.LogInformation($"Este proceso se hizo en {tiempo.ElapsedMilliseconds}ms");

            if (response.IsSuccessStatusCode)
            {
                //se obtiene contenido en formato string
                var contenido =await  response.Content.ReadAsStringAsync();
                //serializarla en clase objeto
                var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize<LibroModeloRemote>(contenido, options);
                var responseAutor= await autorRemote.GetAutor(resultado.AutorLibro?? Guid.Empty);
                if (responseAutor.resultado)
                {
                    var objetoAutor = responseAutor.autor;
                    resultado.AutorData = objetoAutor;
                    var resultadoStr = JsonSerializer.Serialize(resultado);
                    response.Content = new StringContent(resultadoStr, System.Text.Encoding.UTF8,"application/json");
                }
            }



            return response;
        }
    }
}
