using Elastic.Channels;
using Elastic.CommonSchema.Serilog;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace FiapCloudGamesPayments.Api.Extensions
{
    public static class ElasticExtensions
    {
        public static WebApplicationBuilder AddElasticConfiguration(this WebApplicationBuilder builder)
        {
            builder.Services.AddSerilog((context, config) =>
            {
                config
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithCorrelationId()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                    .WriteTo.Console() 
                    .WriteTo.Elasticsearch([new Uri(builder.Configuration["ElasticSearch:Uri"])], opts =>
                    {
                        opts.DataStream = new DataStreamName("logs", builder.Configuration["ElasticSearch:IndexName"], builder.Environment.EnvironmentName);
                        opts.BootstrapMethod = BootstrapMethod.Failure;
                        opts.TextFormatting = new EcsTextFormatterConfiguration<LogEventEcsDocument>();
                        opts.ConfigureChannel = channelOpts =>
                        {
                            channelOpts.BufferOptions = new BufferOptions();
                        };
                    }, transport =>
                    {
                        transport.Authentication(new ApiKey(builder.Configuration["ElasticSearch:ApiKey"]));
                    });
            });

            return builder;
        }
    }
}
