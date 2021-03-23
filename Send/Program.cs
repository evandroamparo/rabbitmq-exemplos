using System;
using RabbitMQ.Client;
using System.Text;
using System.Linq;
using System.Threading;

class Send
{
    private const string EXCHANGE = "eventos";

    public static void Main(string[] args)
    {
        Console.WriteLine("Pressione [enter] para sair");

        bool sair = false;
        do
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(EXCHANGE, ExchangeType.Fanout, durable: true);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);
                var routingKey = GetRoutingKey();

                channel.BasicPublish(EXCHANGE, routingKey, basicProperties: null, body: body);
                Console.WriteLine(" [x] Enviado {0}", routingKey);
            }
            Thread.Sleep(2000);
            sair = Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter;
        } while (!sair);
    }

    private static string GetRoutingKey()
    {
        string[] Keys = { "inclusao", "alteracao", "exclusao" };
        var i = new Random().Next(Keys.Length - 1);
        return Keys[i];
    }
}