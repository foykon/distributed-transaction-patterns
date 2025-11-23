using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Context;
using Order.API.Models;
using Order.API.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM createOrderVM, OrderDbContext dbContext)=>
{

    Order.API.Models.Order order = new Order.API.Models.Order
    {
        CustomerId = createOrderVM.CustomerId,
        OrderStatus = OrderStatus.Suspend,
        OrderDate = DateTime.Now,
        TotalPrice = createOrderVM.OrderItems.Sum(oi=>oi.Count*oi.Price),
        OrderItems = createOrderVM.OrderItems.Select(oi=>new OrderItem
        {
            Price=oi.Price,
            ProductId=oi.ProductId,
            Quantity=oi.Count
        }).ToList()
    };


    await dbContext.Orders.AddAsync(order);
    await dbContext.SaveChangesAsync();

    return Results.Ok("Order created");
});

app.Run();
