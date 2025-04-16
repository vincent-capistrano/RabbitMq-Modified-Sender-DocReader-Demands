using MassTransit;
using Models.Messages;
using Models.Modules.Enums;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Security.Cryptography;

namespace Sender;

public static class SendMessage
{
    public static void Main(string[] args)
    {
        /*
            The variable named 'queueName' below needs to be modified in this code.
            Replace the value of this variable to the queue name where the consumer you are testing is subscribed to.

            IMPORTANT: The endpoint is case-sensitive so make sure the queue name you put here is the EXACT SAME queue name used in the consumer module.
            Hiccups can occur while the bus is starting up, so make sure to test the interaction of the modules multiple times. It helps to reset the queue states via the RabbitMQ Interface for every test.
        */
        string queueName = "Bill";

        // Configure Serilog to write logs to an Elasticsearch sink
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200")))
            .CreateLogger();

        // Configure the MassTransit bus
        IBusControl bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(new Uri("rabbitmq://localhost/"), h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        });

        bus.Start();

        while (true)
        {
            Console.WriteLine($"[Sender]: Destination Queue - [{queueName}]");

            Console.Write("Enter Organization ID (GUID): ");
            string? orgInput = Console.ReadLine();
            if (!Guid.TryParse(orgInput, out Guid orgId))
            {
                Console.WriteLine("Invalid Organization ID. Try again.");
                continue;
            }

            Console.Write("Enter Object ID (GUID) or press ENTER to auto-generate: ");
            string? objInput = Console.ReadLine();
            Guid objId = string.IsNullOrWhiteSpace(objInput) ? Guid.NewGuid() :
                         Guid.TryParse(objInput, out Guid parsedObjId) ? parsedObjId :
                         throw new Exception("Invalid Object ID format");

            Console.Write("Enter Module Type (DocReader or Demands): ");
            string? moduleTypeInput = Console.ReadLine();
            if (!Enum.TryParse(moduleTypeInput, out ModuleType moduleType))
            {
                Console.WriteLine("Invalid ModuleType. Try again.");
                continue;
            }

            var message = new ProcessServiceBill
            {
                ModuleType = moduleType,
                ObjectId = objId,
                OrganizationId = orgId
            };

            Uri sendToUri = new Uri($"rabbitmq://localhost/{queueName}");
            ISendEndpoint endpoint = bus.GetSendEndpoint(sendToUri).Result;
            endpoint.Send(message).Wait();

            Log.Information($"[Sender]: Sent message to [{queueName}] with OrgId [{orgId}], ObjId [{objId}], Module [{moduleType}]");
        }

        bus.Stop();
    }
}