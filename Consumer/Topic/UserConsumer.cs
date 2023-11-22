namespace Consumer.Topic
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class UserConsumer
    {
        private readonly IModel channel;

        public UserConsumer(IModel channel)
        {
            this.channel = channel;
        }

        public void ReceiveMessage()
        {
            this.channel.ExchangeDeclare(exchange: "topic", ExchangeType.Topic);

            this.channel.BasicQos(prefetchSize: 0, prefetchCount: 1,global: false);
            var queue = this.channel.QueueDeclare(queue: "", durable: false, exclusive: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(this.channel);

            consumer.Received += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"User Consumer: {message}");
            };

            this.channel.QueueBind(queue: queue.QueueName, exchange: "topic", routingKey: "user.*.*");

            this.channel.BasicConsume(queue: queue.QueueName, autoAck:true, consumer: consumer);

            Console.ReadKey();
        }
    }
}