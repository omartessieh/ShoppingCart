using Microsoft.EntityFrameworkCore;
using Shop.Models.Models;

namespace Shop.Api.Data
{
    public class ShoppingCartDBContext : DbContext
    {
        public ShoppingCartDBContext(DbContextOptions<ShoppingCartDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminInfo>().ToTable("AdminInfo");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<CustomerOrder>().ToTable("CustomerOrder");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
        }

        public DbSet<AdminInfo> AdminInfos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}