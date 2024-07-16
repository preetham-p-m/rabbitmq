namespace Producer.RequestReply
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal sealed class Client : IProducer
    {
        private const string MessageTemplate = "Request from client {0}";

        private readonly IModel channel;

        public Client(IModel channel)
        {
            this.channel = channel;
        }

        public void SendMessage(int delay)
        {
            Console.WriteLine($"Application started sending the message. Message will be sent every {delay} seconds.");

            var replyQueue = this.channel.QueueDeclare("", exclusive: true);
            this.channel.QueueDeclare("request.queue", exclusive: false);

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (_, ea) =>
            {
                Console.WriteLine($"Reply Received: {ea.BasicProperties.CorrelationId}");
            };
            this.channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

            var properties = this.channel.CreateBasicProperties();
            properties.ReplyTo = replyQueue.QueueName;

            while (true)
            {
                var correlationId = Guid.NewGuid();

                properties.CorrelationId = correlationId.ToString();

                var requestMessage = string.Format(MessageTemplate, correlationId);
                var encodedUserEuropePayments = Encoding.UTF8.GetBytes(requestMessage);

                this.channel.BasicPublish("", "request.queue", properties, encodedUserEuropePayments);
                Console.WriteLine(requestMessage);

                Task.Delay(TimeSpan.FromSeconds(delay)).Wait();

                if (Console.ReadLine() != null)
                {
                    break;
                }
            }

        }
    }
}