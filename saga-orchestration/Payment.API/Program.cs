using MassTransit;
using Payment.API.Consumers;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{

    cfg.AddConsumer<StockReservedEventConsumer>();
    cfg.UsingRabbitMq((context, _cfg) =>
    {
        _cfg.Host(builder.Configuration["RabbitMQ"]);
        _cfg.ReceiveEndpoint(Shared.RabbitMQSettings.Payment_StockReservedEvent, e =>
        {
            e.ConfigureConsumer<StockReservedEventConsumer>(context);
        });
    });
});


var app = builder.Build();

app.Run();
