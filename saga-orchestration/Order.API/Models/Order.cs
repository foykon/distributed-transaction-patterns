namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
