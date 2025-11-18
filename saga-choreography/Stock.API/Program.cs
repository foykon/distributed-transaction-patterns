using MassTransit;
using Shared;
using MongoDB.Driver;

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

using var scope = app.Services.CreateScope();

var mongoDb = scope.ServiceProvider.GetRequiredService<Stock.API.Services.MongoDBService>();

var stockCollection = mongoDb.GetCollection<Stock.API.Models.Stock>();

if(!stockCollection.FindSync(s => true).Any())
{
    await stockCollection.InsertManyAsync(new List<Stock.API.Models.Stock>()
    {
        new Stock.API.Models.Stock(){ ProductId = Guid.NewGuid(), Quantity=10, CreatedDate = DateTime.Now},
        new Stock.API.Models.Stock(){ ProductId = Guid.NewGuid(), Quantity=20, CreatedDate = DateTime.Now},
        new Stock.API.Models.Stock(){ ProductId = Guid.NewGuid(), Quantity=30, CreatedDate = DateTime.Now},
        new Stock.API.Models.Stock(){ ProductId = Guid.NewGuid(), Quantity=40, CreatedDate = DateTime.Now},
        new Stock.API.Models.Stock(){ ProductId = Guid.NewGuid(), Quantity=5, CreatedDate = DateTime.Now},
    });
}

app.Run();
