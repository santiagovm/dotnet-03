using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FlixOne.BookStore.ProductService.Models;
using FlixOne.BookStore.ProductService.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FlixOne.BookStore.ProductService.IntegrationTests
{
    internal static class TestDatabaseUtils
    {
        public static async Task InitDatabase(ProductDbContext productDbContext)
        {
            await InitializeDatabaseServer();
            await CreateDatabase(productDbContext.Database);
            await SeedData(productDbContext);
        }

        private static async Task SeedData(ProductDbContext dbContext)
        {
            var categoryBlue = new Category
                               {
                                   Name = "Blue",
                                   Description = "This is the Blue Category"
                               };
            
            var categoryRed = new Category
                               {
                                   Name = "Red",
                                   Description = "This is the Red Category"
                               };

            var productRed1 = new Product
                              {
                                  Name = "Product Red 1",
                                  Description = "This is the Product Red 1",
                                  Image = "This is the Image for Product Red 1",
                                  Price = 333.33m,
                                  Category = categoryRed
                              };
            
            var productRed2 = new Product
                              {
                                  Name = "Product Red 2",
                                  Description = "This is the Product Red 2",
                                  Image = "This is the Image for Product Red 2",
                                  Price = 444.44m,
                                  Category = categoryRed
                              };
            
            var productBlue1 = new Product
                              {
                                  Name = "Product Blue 1",
                                  Description = "This is the Product Blue 1",
                                  Image = "This is the Image for Product Blue 1",
                                  Price = 222.22m,
                                  Category = categoryBlue
                              };
            
            await dbContext.Categories.AddAsync(categoryRed);
            await dbContext.Categories.AddAsync(categoryBlue);

            await dbContext.Products.AddAsync(productRed1);
            await dbContext.Products.AddAsync(productRed2);
            await dbContext.Products.AddAsync(productBlue1);

            await dbContext.SaveChangesAsync();
        }

        private static async Task CreateDatabase(DatabaseFacade database)
        {
            await database.EnsureDeletedAsync();
            await database.EnsureCreatedAsync();
        }

        private static async Task InitializeDatabaseServer()
        {
            const string image = "mcr.microsoft.com/mssql/server:2017-latest";
            const string containerName = "dotnet-03-int-tests";

            DockerClient dockerClient = new DockerClientConfiguration().CreateClient();

            IList<ContainerListResponse> runningContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters());

            IEnumerable<ContainerListResponse> testContainers =
                runningContainers.Where(container => container.Names.Any(name => name.Contains(containerName)));

            foreach (ContainerListResponse container in testContainers)
            {
                await dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());
                await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters());
            }
            
            await dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
                                                       {
                                                           FromImage = image
                                                       },
                                                       null,
                                                       new Progress<JSONMessage>());

            CreateContainerResponse sqlContainer =
                await dockerClient
                     .Containers
                     .CreateContainerAsync(
                           new CreateContainerParameters
                           {
                               Name = containerName,
                               Image = image,
                               Env = new List<string>
                                     {
                                         "ACCEPT_EULA=Y",
                                         $"SA_PASSWORD={SqlSaPassword}"
                                     },
                               HostConfig = new HostConfig
                                            {
                                                PortBindings = new Dictionary<string, IList<PortBinding>>
                                                                   {
                                                                       {
                                                                           "1433/tcp",
                                                                           new[]
                                                                           {
                                                                               new PortBinding
                                                                               {
                                                                                   HostPort = SqlPort
                                                                               }
                                                                           }
                                                                       }
                                                                   }
                                            }
                           });

            await dockerClient.Containers.StartContainerAsync(sqlContainer.ID, new ContainerStartParameters());

            await WaitUntilDatabaseAvailableAsync(SqlPort);
        }

        private static async Task WaitUntilDatabaseAvailableAsync(string port)
        {
            var connected = false;
            var timedOut = false;

            DateTime timeoutAt = DateTime.UtcNow.AddMinutes(1);
            
            while (!connected && !timedOut)
            {
                timedOut = DateTime.UtcNow > timeoutAt;

                try
                {
                    // todo: set the connection string in appsettings or the web builder, avoid having to have the same values here and appsettings.json
                    var sqlConnString = $"Data Source=localhost,{port};Integrated Security=False;User ID=sa;Password={SqlSaPassword}";
                    await using var sqlConnection = new SqlConnection(sqlConnString);
                    await sqlConnection.OpenAsync();
                    connected = true;
                }
                catch
                {
                    await Task.Delay(500);
                }
            }

            if (!connected)
            {
                throw new Exception($"could not connect to sql server in port [{port}]");
            }
        }
        
        private const string SqlSaPassword = "yourStrong(!)Password";
        private const string SqlPort = "1433";
    }
}
