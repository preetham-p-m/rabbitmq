namespace Consumer.CompetingConsumer
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal sealed class CompetingConsumers : IConsumer
    {
        private readonly IModel channel;
        private readonly Random random;

        public CompetingConsumers(IModel channel)
        {
            this.channel = channel;
            this.random = new Random();
        }

        public void ReceiveMessage()
        {
            this.channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            this.channel.QueueDeclare(queue: "letterbox", exclusive: false, durable: false, arguments: null, autoDelete: false);

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (_, ea) =>
            {
                var processingTime = this.random.Next(8, 20);

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"{message} will take {processingTime} to process");

                Task.Delay(processingTime);
                this.channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            this.channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: consumer);

            Console.WriteLine("Application is ready to receive the message. Press Enter to exit.");
            Console.ReadKey();
        }
    }
}
