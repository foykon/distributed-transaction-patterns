using MassTransit;
using Order.API.Models.Context;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<Shared.Events.PaymentCompletedEvent>
    {
        private readonly OrderAPIDbContext _dbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order =  await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if(order == null)
            {
                throw new Exception("Order not found");
            }
            
                order.OrderStatus = Models.Enums.OrderStatus.Completed;
            await _dbContext.SaveChangesAsync();

        }
    }
}
