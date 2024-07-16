namespace Consumer.Topic
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal sealed class EuropeConsumer : IConsumer
    {
        private readonly IModel channel;

        public EuropeConsumer(IModel channel)
        {
            this.channel = channel;
        }

        public void ReceiveMessage()
        {
            this.channel.ExchangeDeclare(exchange: "topic", ExchangeType.Topic);

            this.channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var queue = this.channel.QueueDeclare(queue: "", durable: false, exclusive: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(this.channel);

            consumer.Received += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Europe Consumer: {message}");
            };

            this.channel.QueueBind(queue: queue.QueueName, exchange: "topic", routingKey: "*.europe.*");

            this.channel.BasicConsume(queue: queue.QueueName, autoAck: true, consumer: consumer);

            Console.ReadKey();
        }
    }
}