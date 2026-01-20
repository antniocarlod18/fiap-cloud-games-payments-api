using FiapCloudGamesPayments.Api.Consumers;
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

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["RabbitMQ:Host"], builder.Configuration["RabbitMQ:VirtualHost"], h =>
                    {
                        h.Username(builder.Configuration["RabbitMQ:UserName"]);
                        h.Password(builder.Configuration["RabbitMQ:Password"]);
                    });

                    cfg.UseMessageRetry(r => r.Immediate(2));
                    cfg.ConfigureEndpoints(context);
                });

                x.AddConsumer<ProcessPaymentConsumer>();
            });

            return builder;
        }
    }
}
