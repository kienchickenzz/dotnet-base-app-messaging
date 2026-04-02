using BaseAppMessaging.Application;
using BaseAppMessaging.Infrastructure;
using BaseAppMessaging.Worker.Extensions;
using BaseAppMessaging.Worker.Consumers.Products;


IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilogFromSettings()
    .ConfigureServices((hostContext, services) =>
    {
        // Shared services from Application layer
        services.AddApplication();

        // Shared services from Infrastructure layer (Messaging only, no Hangfire)
        services.AddInfrastructureForWorker(hostContext.Configuration);

        // Register message consumers as hosted services
        services.AddHostedService<ProductCreatedConsumer>();
    })
    .Build();

host.Run();
