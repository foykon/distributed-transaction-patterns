using Microsoft.EntityFrameworkCore;

namespace Order.API.Models.Context
{
    public class OrderAPIDbContext :DbContext
    {
        public OrderAPIDbContext(DbContextOptions<OrderAPIDbContext> options) : base(options)
        {
        }
        public DbSet<Entities.Order> Orders { get; set; }
        public DbSet<Entities.OrderItem> OrderItems { get; set; }
    }
}
