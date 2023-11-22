namespace Consumer
{
    using RabbitMQ.Client;

    internal class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random();

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine("Consumer Modes:\nCompeting Consumer - 1 \nPubSub Consumer 1 - 2.1 \nPubSub Consumer 2 - 2.2\n");
            Console.Write("Please enter the number of your choice: ");
            var mode = Console.ReadLine();

            switch (mode)
            {
                case "1":
                    new CompetingConsumer(channel).ReceiveMessage();
                    break;

                case "2.1":
                    new PubSubConsumer1(channel).ReceiveMessage();
                    break;

                case "2.2":
                    new PubSubConsumer2(channel).ReceiveMessage();
                    break;
            }
        }
    }
}