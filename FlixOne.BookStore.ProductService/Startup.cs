using FlixOne.BookStore.ProductService.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FlixOne.BookStore.ProductService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            DotNetEnv.Env.TraversePath().Load();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // swagger
            services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Product API", Version = "v1" }));
            
            // db context
            services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(DatabaseConfiguration.ConnectionString));
            
            services.AddScoped<IProductRepository, ProductRepository>();
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1"));
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
