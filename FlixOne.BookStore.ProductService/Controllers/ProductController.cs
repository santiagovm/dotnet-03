using System;
using System.Linq;
using FlixOne.BookStore.ProductService.Models;
using FlixOne.BookStore.ProductService.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace FlixOne.BookStore.ProductService.Controllers
{
    [Route("api/products")]
    public class ProductController : Controller
    {
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            return new OkObjectResult(_productRepository.GetAll().Select(ToProductViewModel).ToList());
        }

        [HttpGet]
        [Route("{productId}")]
        public IActionResult Get(Guid productId)
        {
            Product product = _productRepository.GetBy(productId);

            if (product == null)
            {
                return NotFound();
            }
            
            return new OkObjectResult(ToProductViewModel(product));
        }

        [HttpPost]
        public IActionResult Add([FromBody] ProductRequest productRequest)
        {
            if (productRequest == null)
            {
                return BadRequest();
            }

            Product product = ToProduct(productRequest);
            _productRepository.Add(product);
            
            return new CreatedResult($"{product.Id}", product);
        }

        [HttpPut]
        [Route("{productId}")]
        public IActionResult Update(Guid productId, [FromBody] ProductRequest productRequest)
        {
            if (productRequest == null)
            {
                return BadRequest();
            }

            Product product = _productRepository.GetBy(productId);

            if (product == null)
            {
                return NotFound();
            }

            product.Description = productRequest.ProductDescription;
            product.Name = productRequest.ProductName;
            product.Image = productRequest.ProductImage;
            product.Price = productRequest.ProductPrice;
            product.CategoryId = productRequest.CategoryId;
            
            _productRepository.Update(product);

            return NoContent();
        }

        [HttpDelete]
        [Route("{productId}")]
        public IActionResult Delete(Guid productId)
        {
            Product product = _productRepository.GetBy(productId);

            if (product == null)
            {
                return NotFound();
            }
            
            _productRepository.Remove(productId);

            return NoContent();
        }
        
        private static ProductViewModel ToProductViewModel(Product productModel)
        {
            return new ProductViewModel
                   {
                       CategoryId = productModel.CategoryId,
                       CategoryDescription = productModel.Category.Description,
                       CategoryName = productModel.Category.Name,
                       ProductDescription = productModel.Description,
                       ProductId = productModel.Id,
                       ProductImage = productModel.Image,
                       ProductName = productModel.Name,
                       ProductPrice = productModel.Price
                   };
        }

        private static Product ToProduct(ProductRequest productRequest)
        {
            return new Product
                   {
                       CategoryId = productRequest.CategoryId,
                       Description = productRequest.ProductDescription,
                       Id = default,
                       Name = productRequest.ProductName,
                       Price = productRequest.ProductPrice
                   };
        }
        
        private readonly IProductRepository _productRepository;
    }
}
