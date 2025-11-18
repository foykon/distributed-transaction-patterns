using MassTransit;
using Shared;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _publishEndponit;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public StockReservedEventConsumer(IPublishEndpoint publishEndponit, ISendEndpointProvider sendEndpointProvider)
        {
            _publishEndponit = publishEndponit;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var paymentSucceeded = new Random().Next(1, 10) <= 8; // 80% chance of success
            if (paymentSucceeded)
            {
                Console.WriteLine($"Payment succeeded for OrderId: {context.Message.OrderId}, TotalPrice: {context.Message.TotalPrice}");
                await _publishEndponit.Publish<PaymentCompletedEvent>(new PaymentCompletedEvent()
                {   
                    OrderId =  context.Message.OrderId
                });
            }
            else
            {
                Console.WriteLine($"Payment failed for OrderId: {context.Message.OrderId}, TotalPrice: {context.Message.TotalPrice}");
                await _publishEndponit.Publish<PaymentFailedEvent>(new PaymentFailedEvent()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    OrderItems = context.Message.OrderItemMessages,
                    Message = "Payment processing failed due to insufficient funds."
                });
            }
        }
    }
}
