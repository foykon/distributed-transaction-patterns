using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Context;
using Order.API.Models.Entities;
using Order.API.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((context, _cfg) =>
    {
        _cfg.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddDbContext<Order.API.Models.Context.OrderAPIDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM model, OrderAPIDbContext context) =>
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

    return Results.Ok(order);
});

app.Run();
