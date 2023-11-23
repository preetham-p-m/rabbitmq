namespace Consumer.RequestReply
{
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal sealed class Server
    {
        private const string ReplyTemplate = "Replay to : {0}";
        private readonly IModel channel;

        public Server(IModel channel)
        {
            this.channel = channel;
        }

        public void StartProcessing()
        {
            var queue = this.channel.QueueDeclare("request.queue", exclusive: false);

            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += (_, ea) =>
            {

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"{message} \ncorrelation-id = {ea.BasicProperties.CorrelationId}");

                var replyMessage = string.Format(ReplyTemplate, ea.BasicProperties.CorrelationId);
                var encodedReplyMessage = Encoding.UTF8.GetBytes(replyMessage);

                this.channel.BasicPublish("", ea.BasicProperties.ReplyTo, null, encodedReplyMessage);
            };

            this.channel.BasicConsume(queue: queue.QueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Application is ready to receive the message. Press Enter to exit.");
            Console.ReadKey();
        }
    }
}