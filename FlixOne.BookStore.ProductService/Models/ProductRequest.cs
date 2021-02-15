using System;

namespace FlixOne.BookStore.ProductService.Models
{
    // todo: make immutable
    public class ProductRequest
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public Guid CategoryId { get; set; }
    }
}
