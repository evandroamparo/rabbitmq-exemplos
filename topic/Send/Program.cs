using System;
using RabbitMQ.Client;
using System.Text;

class Send
{
    private const string EXCHANGE = "eventos_topic";
    private static string[] TiposEventos = { "inclusao", "alteracao", "exclusao" };

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: {0} [fila]",
                                    Environment.GetCommandLineArgs()[0]);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
            Environment.ExitCode = 1;
            return;
        }

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {

            var fila = args[0];
            var tipoEvento = GetTipoEvento();
            var routingKey = $"{fila}.{tipoEvento}";

            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Topic, durable: true);

            const string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            string queue = routingKey.Split(".")[0];
            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue, EXCHANGE, routingKey: $"{fila}.*");
            System.Console.WriteLine($"Fila: {queue}");

            channel.BasicPublish(EXCHANGE,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
        }
    }

    private static string GetTipoEvento()
    {
        var i = new Random().Next(TiposEventos.Length);
        return TiposEventos[i];
    }
}