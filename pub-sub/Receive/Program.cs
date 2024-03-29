﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Receive
{
    private const string EXCHANGE = "eventos";

    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(EXCHANGE, ExchangeType.Fanout, durable: true);

            string idFila = args.Length > 0 ? args[0] : "";

            var fila = channel.QueueDeclare(idFila, durable: true, exclusive: false, autoDelete: false);

            channel.QueueBind(idFila, EXCHANGE, routingKey: "");

            Console.WriteLine($" [*] Aguardando mensagens na fila {idFila}.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Recebido {0}: {1}", ea.RoutingKey, message);
            };
            channel.BasicConsume(fila.QueueName, autoAck: true, consumer: consumer);

            Console.WriteLine(" Pressione [enter] para sair.");
            Console.ReadLine();
        }
    }
}