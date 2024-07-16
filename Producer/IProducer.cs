namespace Producer
{
    public interface IProducer
    {
        void SendMessage(int delay);
    }
}