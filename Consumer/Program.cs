namespace Consumer
{
    using PubSub;
    using CompetingConsumer;
    using RabbitMQ.Client;
    using RequestReply;
    using Topic;

    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine("Consumer Modes: \n"
                + "Competing Consumer - 1 \n"
                + "PubSub Consumer 1 - 2.1 \n"
                + "PubSub Consumer 2 - 2.2 \n"
                + "Topic Consumer {user.*.*} - 3.1 \n"
                + "Topic Consumer 2 {*.europe.*} - 3.2 \n"
                + "Topic Consumer 3 {#.payments} - 3.3 \n"
                + "Server - 4\n");

            Console.Write("Please enter the number of your choice: ");
            var mode = Console.ReadLine();

            switch (mode)
            {
                case "1":
                    new CompetingConsumers(channel).ReceiveMessage();
                    break;

                case "2.1":
                    new PubSubConsumer1(channel).ReceiveMessage();
                    break;

                case "2.2":
                    new PubSubConsumer2(channel).ReceiveMessage();
                    break;

                case "3.1":
                    new UserConsumer(channel).ReceiveMessage();
                    break;

                case "3.2":
                    new EuropeConsumer(channel).ReceiveMessage();
                    break;

                case "3.3":
                    new PaymentsConsumer(channel).ReceiveMessage();
                    break;

                case "4":
                    new Server(channel).StartProcessing();
                    break;
            }
        }
    }
}