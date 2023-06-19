using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.DTO;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;

        private ServiceBusProcessor _processor;
        public AzureServiceBusConsumer(IConfiguration configuration)
        {

            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue= _configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCart");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _processor = client.CreateProcessor(emailCartQueue);
            /*_processor.ProcessMessageAsync*/

        }

        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnEmailCartReceived;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }

        private async Task OnEmailCartReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDTO objMessage=JsonConvert.DeserializeObject<CartDTO>(body);
            try 
            {
                // Todo try to log the email
                await arg.CompleteMessageAsync(arg.Message);    
            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
