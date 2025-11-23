namespace Order.API.ViewModels
{
    public class CreateOrderVM
    {
        public int CustomerId { get; set; }
        public List<OrderItemVM> OrderItems { get; set; }
    }
}
