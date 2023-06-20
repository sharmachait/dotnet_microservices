using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.DTO;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer:IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _processor;
        private ServiceBusProcessor _processorUserRegistration;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService = emailService;

            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");

            emailCartQueue= _configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCart");

            emailUserQueue = _configuration.GetValue<string>("TopicAndQueueName:EmailUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _processor = client.CreateProcessor(emailCartQueue);
            _processorUserRegistration=client.CreateProcessor(emailUserQueue);
            /*_processor.ProcessMessageAsync*/

        }

        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnEmailCartReceived;
            _processorUserRegistration.ProcessMessageAsync += OnEmailUserRegister;

            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();

            _processorUserRegistration.ProcessErrorAsync += ErrorHandler;
            await _processorUserRegistration.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
            await _processorUserRegistration.StopProcessingAsync();
            await _processorUserRegistration.DisposeAsync();

        }

        private async Task OnEmailCartReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDTO objMessage=JsonConvert.DeserializeObject<CartDTO>(body);
            try 
            {
                //we cant use a scoped service inside of a singleton service
                //we need to change the db context in order to be able to use it inside this class just for this 

                await _emailService.EmailCartAndLog(objMessage);
                await arg.CompleteMessageAsync(arg.Message);    
            }
            catch(Exception ex) 
            {
                throw;
            }
        }        
        private async Task OnEmailUserRegister(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string objMessage=JsonConvert.DeserializeObject<string>(body);
            try 
            {
                //we cant use a scoped service inside of a singleton service
                //we need to change the db context in order to be able to use it inside this class just for this 

                await _emailService.EmailUserRegister(objMessage);
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
