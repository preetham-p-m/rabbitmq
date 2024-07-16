namespace Producer.Topic
{
    using System.Text;
    using RabbitMQ.Client;

    internal sealed class TopicPublisher : IProducer
    {
        private const string MessageTemplate = "Topic Message {0}";

        private readonly IModel channel;

        public TopicPublisher(IModel channel)
        {
            this.channel = channel;
        }

        public void SendMessage(int delay)
        {
            this.channel.ExchangeDeclare("topic", ExchangeType.Topic);
            Console.WriteLine($"Application started sending the message. Message will be sent every {delay} seconds.");

            while (true)
            {
                var userEuropePayments = string.Format(MessageTemplate, "user.europe.payments");
                var encodedUserEuropePayments = Encoding.UTF8.GetBytes(userEuropePayments);

                this.channel.BasicPublish("topic", "user.europe.payments", null, encodedUserEuropePayments);
                Console.WriteLine(userEuropePayments);

                var europePayments = string.Format(MessageTemplate, "europe.payments");
                var encodedEuropePayments = Encoding.UTF8.GetBytes(europePayments);

                this.channel.BasicPublish("topic", "adds.europe.payments", null, encodedEuropePayments);
                Console.WriteLine(europePayments);

                Task.Delay(TimeSpan.FromSeconds(delay)).Wait();
            }
        }
    }
}