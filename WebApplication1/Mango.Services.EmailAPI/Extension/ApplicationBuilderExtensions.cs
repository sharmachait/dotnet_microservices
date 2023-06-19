

using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer serviceBusConsumer{ get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app) {
            serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();

            var hostapplifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostapplifetime.ApplicationStarted.Register(OnStart);
            hostapplifetime.ApplicationStopping.Register(OnStop);
            return app;
        }
        public static void OnStart() {
            serviceBusConsumer.Start();
        }
        public static void OnStop() {
            serviceBusConsumer.Stop();
        }
    }
}
