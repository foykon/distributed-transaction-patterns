using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class RabbitMQSettings
    {
        public const string Stcok_OrderCreatedEvent = "stock-order-created-event";
        public const string Order_StockNotReservedEvent = "order-stock-not-reserved-event";
        public const string Order_PaymentFailedEvent = "order-payment-failed-event";
        public const string Order_PaymentCompletedEvent = "order-payment-completed-event";
        public const string Stock_PaymentFailedEvent = "stock-payment-failed-event";
        public const string Payment_StockReservedEvent = "payment-stock-reserved-event";
    }
}
