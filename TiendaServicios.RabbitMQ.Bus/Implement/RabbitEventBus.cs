using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.RabbitMQ.Bus.BusRabbit;
using TiendaServicios.RabbitMQ.Bus.Comandos;
using TiendaServicios.RabbitMQ.Bus.Eventos;

namespace TiendaServicios.RabbitMQ.Bus.Implement
{
    public class RabbitEventBus : IRabbitEventBus
    {
        private readonly IMediator mediator;
        private readonly Dictionary<string, List<Type>> manejadores;
        private readonly List<Type> eventoTipos;
        public RabbitEventBus(IMediator _mediator)
        {
            mediator = _mediator;
            manejadores = new Dictionary<string, List<Type>>();
            eventoTipos = new List<Type>();
        }
        public Task AnviarComando<T>(T comando) where T : Comando
        {
            return mediator.Send(comando);
        }

        public void Publish<T>(T evento) where T : Evento
        {
            var factory = new ConnectionFactory() { HostName = "rabbit-spool-web" }; //si esta en container, debe ser el nomobre del docker container
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                var eventName = evento.GetType().Name;

                channel.QueueDeclare(eventName, false, false, false, null);

                var message = JsonConvert.SerializeObject(evento);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", eventName, null, body);
            }
        }

        public void Suscribe<T, TH>()
            where T : Evento
            where TH : IEventoManejador<T>
        {
            var eventName = typeof(T).Name;

            var manejadorEventoTipo = typeof(TH);

            if (!eventoTipos.Contains(typeof(T)))
                eventoTipos.Add(typeof(T));

            if (!manejadores.ContainsKey(eventName))
            {
                manejadores.Add(eventName, new List<Type>());
            }

            if (manejadores[eventName].Any(x => x.GetType() == manejadorEventoTipo))
            {
                throw new ArgumentException($"el manejador {manejadorEventoTipo.Name} fue registrado anteriormente por {eventName}");

            }

            manejadores[eventName].Add(manejadorEventoTipo);

            var factory = new ConnectionFactory() { HostName = "rabbit-spool-web", DispatchConsumersAsync = true };

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += Consumer_Delegate;

            channel.BasicConsume(eventName, true, consumer);

        }

        private async Task Consumer_Delegate(object sender, BasicDeliverEventArgs e)
        {
            var nombreEvento = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                if (manejadores.ContainsKey(nombreEvento))
                {
                    var subscriptions = manejadores[nombreEvento];
                    foreach(var sb in subscriptions)
                    {
                        var manejador = Activator.CreateInstance(sb);
                        
                        if (manejador == null) continue;

                        var tipoevento = eventoTipos.SingleOrDefault(x => x.Name == nombreEvento);
                        var eventoDS = JsonConvert.DeserializeObject(message, tipoevento);

                        var concretoTipo = typeof(IEventoManejador<>).MakeGenericType(tipoevento);

                        await (Task)concretoTipo.GetMethod("Handle").Invoke(manejador, new object[] { eventoDS});
                    }
                }
            }catch(Exception ex)
            {

            }
        }
    }
}
