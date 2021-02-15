using System;
using System.Collections.Generic;
using System.Linq;
using FlixOne.BookStore.ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlixOne.BookStore.ProductService.Persistence
{
    public class ProductRepository : IProductRepository
    {
        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Add(Product product)
        {
            _dbContext.Add(product);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Product> GetAll()
        {
            return _dbContext.Products.Include(navPath => navPath.Category).ToList();
        }

        public Product GetBy(Guid id)
        {
            return _dbContext.Products.Include(navPath => navPath.Category).FirstOrDefault(p => p.Id == id);
        }

        public void Remove(Guid id)
        {
            Product product = GetBy(id);
            _dbContext.Remove(product);
            _dbContext.SaveChanges();
        }

        public void Update(Product product)
        {
            _dbContext.Update(product);
            _dbContext.SaveChanges();
        }
        
        private readonly ProductDbContext _dbContext;
    }
}
