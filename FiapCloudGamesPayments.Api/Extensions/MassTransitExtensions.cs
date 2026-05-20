using FiapCloudGamesPayments.Api.Consumers;
using FiapCloudGamesPayments.Api.Filters;
using MassTransit;

namespace FiapCloudGamesPayments.Api.Extensions
{
    public static class MassTransitExtensions
    {
        public static WebApplicationBuilder AddMassTransitConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(prefix: builder.Environment.EnvironmentName, includeNamespace: false));

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.UseSendFilter(typeof(TracingSendFilter<>), context);
                    cfg.UsePublishFilter(typeof(TracingPublishFilter<>), context);

                    cfg.UseConsumeFilter(typeof(TracingConsumeFilter<>), context);

                    var connectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new InvalidOperationException(
                            "Configure AzureServiceBus:ConnectionString with the Service Bus namespace connection string (Azure Portal → namespace → Shared access policies).");
                    }

                    cfg.Host(connectionString);

                    cfg.UseMessageRetry(r => r.Immediate(2));
                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<ProcessPaymentConsumer>();
            });

            return builder;
        }
    }
}
