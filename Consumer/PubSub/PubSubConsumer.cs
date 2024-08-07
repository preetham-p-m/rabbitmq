namespace Consumer.PubSub
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal sealed class PubSubConsumer : IConsumer
    {
        private readonly IModel channel;

        private readonly string consumerName;

        public PubSubConsumer(IModel channel, string consumerName)
        {
            this.channel = channel;
            this.consumerName = consumerName;
        }

        public void ReceiveMessage()
        {
            this.channel.ExchangeDeclare("Pub.Sub", ExchangeType.Fanout);

            this.channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var queue = this.channel.QueueDeclare(queue: "", durable: false, exclusive: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(this.channel);

            consumer.Received += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"{this.consumerName}: {message}");
            };

            this.channel.QueueBind(queue: queue.QueueName, exchange: "Pub.Sub", routingKey: "");

            this.channel.BasicConsume(queue: queue.QueueName, autoAck: true, consumer: consumer);

            Console.ReadKey();

            this.channel.QueueDelete(queue.QueueName);
        }
    }
}