namespace Producer
{
    using CompetingConsumer;
    using PubSub;
    using RabbitMQ.Client;
    using RequestReply;
    using Topic;

    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine("Producer Modes: \n"
                + "Competing Producer - 1 \n"
                + "PubSub Producer - 2 \n"
                + "Topic Producer - 3 \n"
                + "Client - 4\n");

            Console.Write("Please enter the number of your choice: ");
            var mode = Console.ReadLine();

            const int producerDelay = 3;

            IProducer producer = mode switch
            {
                "1" => new Producer(channel),
                "2" => new PubSubProducer(channel),
                "3" => new TopicPublisher(channel),
                "4" => new Client(channel),
                _ => throw new ArgumentException("Invalid Producer")
            };

            producer.SendMessage(producerDelay);

        }
    }
}