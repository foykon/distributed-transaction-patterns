using MassTransit;
using Stock.API.Services;
using MongoDB.Driver;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<Shared.Events.PaymentFailedEvent>
    {
        private readonly MongoDBService _mongoDBService;

        public PaymentFailedEventConsumer(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task Consume(ConsumeContext<Shared.Events.PaymentFailedEvent> context)
        {
            var stocks = _mongoDBService.GetCollection<Models.Stock>();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock =  await (await stocks.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();
                if(stock!=null){ stock.Quantity += orderItem.Quantity; }
                await stocks.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
            }


        }
    
    }
}
