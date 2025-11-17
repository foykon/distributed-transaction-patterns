using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((context, _cfg) =>
    {
        _cfg.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddSingleton<Stock.API.Services.MongoDBService>();

var app = builder.Build();

app.Run();
