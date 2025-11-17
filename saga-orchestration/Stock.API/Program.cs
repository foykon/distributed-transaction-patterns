using MassTransit;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<Stock.API.Consumers.OrderCreatedEventConsumer>();
    cfg.AddConsumer<Stock.API.Consumers.PaymentFailedEventConsumer>();

    cfg.UsingRabbitMq((context, _cfg) =>
    {
        _cfg.Host(builder.Configuration["RabbitMQ"]);

        _cfg.ReceiveEndpoint(RabbitMQSettings.Stcok_OrderCreatedEvent, e =>
        {
            e.ConfigureConsumer<Stock.API.Consumers.OrderCreatedEventConsumer>(context);
        });
        _cfg.ReceiveEndpoint(RabbitMQSettings.Stock_PaymentFailedEvent, e =>
        {
            e.ConfigureConsumer<Stock.API.Consumers.PaymentFailedEventConsumer>(context);
        });
    });
});

builder.Services.AddSingleton<Stock.API.Services.MongoDBService>();

var app = builder.Build();

app.Run();
