using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FlixOne.BookStore.ProductService.Models;
using FlixOne.BookStore.ProductService.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FlixOne.BookStore.ProductService.IntegrationTests.Services
{
    public class ProductTest
    {
        public ProductTest()
        {
            IWebHostBuilder testWebHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            var testServer = new TestServer(testWebHostBuilder);
            _testHttpClient = testServer.CreateClient();
            _productDbContext = testServer.Services.GetService<ProductDbContext>();
        }

        [Fact]
        public async Task ReturnProductList()
        {
            // arrange
            await TestDatabaseUtils.InitDatabase(_productDbContext);

            // act
            HttpResponseMessage response = await _testHttpClient.GetAsync("api/products");
            
            // assert
            response.EnsureSuccessStatusCode();

            ProductViewModel[] products = await JsonSerializer.DeserializeAsync<ProductViewModel[]>(
                 await response.Content.ReadAsStreamAsync(),
                 new JsonSerializerOptions(JsonSerializerDefaults.Web)
                );
            
            Assert.NotNull(products);
            Assert.NotEmpty(products);
        }
        
        private readonly HttpClient _testHttpClient;
        private readonly ProductDbContext _productDbContext;
    }
}
