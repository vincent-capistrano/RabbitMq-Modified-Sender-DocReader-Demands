using System;
using MassTransit;
using Models.Messages;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Consumer;

public static class Program
{
    public static void Main(string[] args)
    {
        string queueName = "QueueName";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200")))
            .CreateLogger();

        IBusControl bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(new Uri("rabbitmq://localhost/"), h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.Consumer<ProcessCollectionConsumer>();
            });
        });

        bus.Start();
        Console.WriteLine("[Consumer]: Listening for messages. Press Enter to exit.");
        Console.ReadLine();
        bus.Stop();
    }
}