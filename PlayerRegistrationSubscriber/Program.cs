using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            string exchangeName = "PlayerInformation";
            var queueName = channel.QueueDeclare().QueueName;

            // Bind queue to both registration and achievement events
            var registrationBinding = new Dictionary<string, object> { { "type", "registration" } };
            var achievementBinding = new Dictionary<string, object> { { "type", "achievements" } };

            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty, arguments: registrationBinding);
            channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty, arguments: achievementBinding);

            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}