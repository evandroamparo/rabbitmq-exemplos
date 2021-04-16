using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Receive
{
    private const string EXCHANGE = "eventos_topic";

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
            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Topic, durable: true);


            var queueName = args[0];
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: true);

            channel.QueueBind(queueName,
                              EXCHANGE,
                              routingKey: $"{queueName}.*");
            System.Console.WriteLine($"Fila: {queueName}");

            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                Console.WriteLine(" [x] Received '{0}':'{1}'",
                                  routingKey,
                                  message);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}