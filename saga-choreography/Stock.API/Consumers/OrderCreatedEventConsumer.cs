using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Models;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly MongoDBService _mongoDBService;
        private readonly ISendEndpointProvider _sendEndpoint;
        private readonly IPublishEndpoint _publishEndponit;

        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpoint, IPublishEndpoint publishEndponit)
        {
            _mongoDBService = mongoDBService;
            _sendEndpoint = sendEndpoint;
            _publishEndponit = publishEndponit;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new List<bool>();

            IMongoCollection<Models.Stock> collection =  _mongoDBService.GetCollection<Models.Stock>();
            foreach(var orderItem in context.Message.OrderItemMessages)
            {
                stockResult.Add( await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Quantity >= orderItem.Quantity)).AnyAsync());
            }    

            if(stockResult.TrueForAll(s => s == true))
            {
                foreach(var orderItem in context.Message.OrderItemMessages)
                {
                    var stock =(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync());
                    stock.Quantity -= orderItem.Quantity;
                    await collection.FindOneAndReplaceAsync(s=>s.ProductId == orderItem.ProductId, stock);
                }
                ISendEndpoint sendEndpoint = await _sendEndpoint.GetSendEndpoint(new Uri($"queue: {RabbitMQSettings.Payment_StockReservedEvent}"));
                await sendEndpoint.Send(new StockReservedEvent()
                {            
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItemMessages = context.Message.OrderItemMessages
                });
            }
            else
            {
                List<Guid> nonStcokProductIds = new List<Guid>();
                for (int i =0; i< stockResult.Count; i++)
                {
                    if(stockResult[i] == false)
                    {
                        nonStcokProductIds.Add(context.Message.OrderItemMessages[i].ProductId);
                    }
                }

                await _publishEndponit.Publish(new StockNotReservedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = nonStcokProductIds.Count > 0 ? $"Stock not available for products: {string.Join(", ", nonStcokProductIds)}" : "Stock not available"
                });
            }
        }
    }
}
