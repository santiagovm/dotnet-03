using FlixOne.BookStore.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlixOne.BookStore.ProductService.Persistence
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
            
        }

        public ProductDbContext()
        {
            
        }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
