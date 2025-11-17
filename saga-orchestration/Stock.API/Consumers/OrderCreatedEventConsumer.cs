using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Stock.API.Models;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly MongoDBService _mongoDBService;

        public OrderCreatedEventConsumer(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
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
                // call payment
            }
            else
            {
                // stock failed
                //call order cancel event
            }
        }
    }
}
