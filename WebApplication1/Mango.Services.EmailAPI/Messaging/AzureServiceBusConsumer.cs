using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
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

        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subscription;
        private ServiceBusProcessor emailOrderPlacedProcessor;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _emailService = emailService;

            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue= _configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCart");
            emailUserQueue = _configuration.GetValue<string>("TopicAndQueueName:EmailUserQueue");

            orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueName:OrderCreatedTopic");
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueName:OrderCreated_Rewards_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _processor = client.CreateProcessor(emailCartQueue);
            _processorUserRegistration=client.CreateProcessor(emailUserQueue);

            emailOrderPlacedProcessor = client.CreateProcessor(orderCreated_Topic,orderCreated_Email_Subscription);
            
        }

        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnEmailCartReceived;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();

            _processorUserRegistration.ProcessMessageAsync += OnEmailUserRegister;
            _processorUserRegistration.ProcessErrorAsync += ErrorHandler;
            await _processorUserRegistration.StartProcessingAsync();

            emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequesReceived;
            emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
            await emailOrderPlacedProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();

            await _processorUserRegistration.StopProcessingAsync();
            await _processorUserRegistration.DisposeAsync();

            await emailOrderPlacedProcessor.StopProcessingAsync();
            await emailOrderPlacedProcessor.DisposeAsync();

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

        private async Task OnOrderPlacedRequesReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                //we cant use a scoped service inside of a singleton service
                //we need to change the db context in order to be able to use it inside this class just for this 

                await _emailService.LogOrderPlaced(objMessage);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
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
