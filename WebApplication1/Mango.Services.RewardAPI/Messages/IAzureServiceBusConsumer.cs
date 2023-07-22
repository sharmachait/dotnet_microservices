namespace Mango.Services.RewardAPI.Messages
{
    public interface IAzureServiceBusConsumer
    {
        Task Start();
        Task Stop(); 
    }
}
