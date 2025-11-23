using MassTransit;
using Stock.API.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddSingleton<MongoDbService>();


var app = builder.Build();


using var scope = app.Services.CreateScope();

var mongoDb = scope.ServiceProvider.GetRequiredService<Stock.API.Services.MongoDbService>();

var stockCollection = mongoDb.GetCollection<Stock.API.Models.Stock>();

if (!stockCollection.FindSync(s => true).Any())
{
    await stockCollection.InsertManyAsync(new List<Stock.API.Models.Stock>()
    {
        new Stock.API.Models.Stock(){ ProductId = 1, Quantity=100 },
        new Stock.API.Models.Stock(){ ProductId = 2, Quantity=150 },
        new Stock.API.Models.Stock(){ ProductId = 3, Quantity=200 },
        new Stock.API.Models.Stock(){ ProductId = 4, Quantity=250 },
        new Stock.API.Models.Stock(){ ProductId = 5, Quantity=300 },
    });
}

app.Run();
