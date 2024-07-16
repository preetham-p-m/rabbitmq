namespace Producer.CompetingConsumer
{
    using System.Text;
    using RabbitMQ.Client;

    internal sealed class Producer
    {
        private const string MessageTemplate = "Message Number: {0}";

        private int _messageNumber = 1;

        private readonly IModel channel;

        public Producer(IModel channel)
        {
            this.channel = channel;
        }

        public void SendMessage(int delay)
        {
            this.channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine($"Application started sending the message. Message will be sent every {delay} seconds. Press Enter to exit.");

            while (true)
            {
                var message = string.Format(MessageTemplate, _messageNumber++);
                var encodedMessage = Encoding.UTF8.GetBytes(message);
                this.channel.BasicPublish("", "letterbox", null, encodedMessage);

                Console.WriteLine(message);

                Task.Delay(TimeSpan.FromSeconds(delay)).Wait();

                if (Console.ReadLine() != null)
                {
                    break;
                }
            }
        }
    }
}