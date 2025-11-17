using MassTransit;
using Order.API.Models.Context;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly OrderAPIDbContext _dbContext;

        public PaymentFailedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
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
