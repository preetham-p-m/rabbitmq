namespace Producer.PubSub
{
    using System.Text;
    using RabbitMQ.Client;

    internal sealed class PubSubProducer
    {
        private const string MessageTemplate = "Pubsub Message {0}";

        private static int _messageNumber = 1;

        private readonly IModel channel;


        public PubSubProducer(IModel channel)
        {
            this.channel = channel;
        }

        public void SendMessage(int delay)
        {
            this.channel.ExchangeDeclare("pub_sub", ExchangeType.Fanout);
            Console.WriteLine($"Application started sending the message. Message will be sent every {delay} seconds.");

            while (true)
            {
                var message = string.Format(MessageTemplate, _messageNumber++);
                var encodedMessage = Encoding.UTF8.GetBytes(message);

                this.channel.BasicPublish("Pub.Sub", "", null, encodedMessage);
                Console.WriteLine(message);

                Task.Delay(TimeSpan.FromSeconds(delay)).Wait();
            }
        }
    }
}