using System;
using System.Collections.Generic;

namespace FlixOne.BookStore.ProductService
{
    public static class DatabaseConfiguration
    {
        public static string ConnectionString { get; }

        static DatabaseConfiguration()
        {
            string databaseHost = Environment.GetEnvironmentVariable("DOTNET_03_DATABASE_HOST");
            string databasePortString = Environment.GetEnvironmentVariable("DOTNET_03_DATABASE_PORT");
            string databaseUsername = Environment.GetEnvironmentVariable("DOTNET_03_DATABASE_USERNAME");
            string databasePassword = Environment.GetEnvironmentVariable("DOTNET_03_DATABASE_PASSWORD");
            string databaseName = Environment.GetEnvironmentVariable("DOTNET_03_DATABASE_NAME");

            var missingVariables = new List<string>();

            if (string.IsNullOrWhiteSpace(databaseHost))
            {
                missingVariables.Add("DOTNET_03_DATABASE_HOST");
            }
            
            if (string.IsNullOrWhiteSpace(databasePortString))
            {
                missingVariables.Add("DOTNET_03_DATABASE_PORT");
            }
            
            if (string.IsNullOrWhiteSpace(databaseUsername))
            {
                missingVariables.Add("DOTNET_03_DATABASE_USERNAME");
            }
            
            if (string.IsNullOrWhiteSpace(databasePassword))
            {
                missingVariables.Add("DOTNET_03_DATABASE_PASSWORD");
            }
            
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                missingVariables.Add("DOTNET_03_DATABASE_NAME");
            }
            
            var errors = new List<string>();
            
            if (missingVariables.Count > 0)
            {
                errors.Add($"missing environment variables: {string.Join(",", missingVariables)}");
            }

            int databasePort = -1;
                
            if (!string.IsNullOrWhiteSpace(databasePortString) && !int.TryParse(databasePortString, out databasePort))
            {
                errors.Add("DOTNET_03_DATABASE_PORT must be a number");
            }

            if (errors.Count > 0)
            {
                throw new Exception(string.Join(",", errors));
            }
            
            ConnectionString =
                $"Data Source={databaseHost},{databasePort}" +
                $";User ID={databaseUsername}" + 
                $";Password={databasePassword}" +
                $";Initial Catalog={databaseName}" +
                ";Integrated Security=False" +
                ";MultipleActiveResultSets=True";
        }
    }
}
