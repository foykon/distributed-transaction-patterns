using MassTransit;
using Order.API.Models.Context;
using Shared.Events;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly OrderAPIDbContext _dbContext;
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            order.OrderStatus = Models.Enums.OrderStatus.Fail;
            await _dbContext.SaveChangesAsync();

        }
    }
}
