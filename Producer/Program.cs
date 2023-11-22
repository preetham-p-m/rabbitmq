namespace Producer
{
    using RabbitMQ.Client;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            Console.WriteLine("Producer Modes:\nCompeting Producer - 1 \nPubSub Producer - 2\n");
            Console.Write("Please enter the number of your choice: ");
            var mode = Console.ReadLine();

            var producerDelay = 3;

            switch (mode)
            {
                case "1":
                    new Producer(channel).SendMessage(producerDelay);
                    break;

                case "2":
                    new PubSubProducer(channel).SendMessage(producerDelay);
                    break;
            }

        }
    }
}