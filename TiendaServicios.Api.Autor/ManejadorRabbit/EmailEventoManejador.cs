using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.EventoQueue;

namespace TiendaServicios.Api.Autor.ManejadorRabbit
{
    public class EmailEventoManejador : IEventoManejador<EmailEventoQueue>
    {
        //public readonly ILogger<EmailEventoManejador> logger;
        public EmailEventoManejador() { }
        //public EmailEventoManejador(ILogger<EmailEventoManejador> _logger)
        //{
        //    logger = _logger;
        //}
        public Task Handle(EmailEventoQueue @event)
        {
           // logger.LogInformation($"este el valor que consumo desde rabbitmq {@event.Titulo}");

            return Task.CompletedTask;
        }
    }
}
