using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Context;
using Order.API.Models.Entities;
using Order.API.ViewModels;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<Order.API.Consumers.PaymentCompletedEventConsumer>();
    cfg.AddConsumer<Order.API.Consumers.PaymentFailedEventConsumer>();
    cfg.AddConsumer<Order.API.Consumers.StockNotReservedEventConsumer>();
    cfg.UsingRabbitMq((context, _cfg) =>
    {
        _cfg.Host(new Uri(builder.Configuration["RabbitMQ:Url"]));
        _cfg.ReceiveEndpoint(Shared.RabbitMQSettings.Order_PaymentCompletedEvent, e =>
        {
            e.ConfigureConsumer<Order.API.Consumers.PaymentCompletedEventConsumer>(context);
        });
        _cfg.ReceiveEndpoint(Shared.RabbitMQSettings.Order_PaymentFailedEvent, e =>
        {
            e.ConfigureConsumer<Order.API.Consumers.PaymentFailedEventConsumer>(context);
        });
        _cfg.ReceiveEndpoint(Shared.RabbitMQSettings.Order_StockNotReservedEvent, e =>
        {
            e.ConfigureConsumer<Order.API.Consumers.StockNotReservedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<Order.API.Models.Context.OrderAPIDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM model, OrderAPIDbContext context, IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Entities.Order order = new()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : Guid.NewGuid(),
        CreatedDate = DateTime.UtcNow,
        OrderItems = model.OrderItems.Select(oi => new OrderItem(){
            Quantity = oi.Quantity,
            ProductId = Guid.TryParse(oi.ProductId, out Guid _productId) ? _productId : Guid.NewGuid(),
            ProductName = "product" + oi.ProductId[0]

        }).ToList(),
        OrderStatus = Order.API.Models.Enums.OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Quantity)

    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItemMessages = order.OrderItems.Select(oi=> new Shared.Messages.OrderItemMessage()
        {
            Price = oi.UnitPrice,
            ProductId = oi.ProductId,
            Quantity = oi.Quantity
        }).ToList()
    };

    await publishEndpoint.Publish(orderCreatedEvent);

    return Results.Ok(order);
});

app.Run();
