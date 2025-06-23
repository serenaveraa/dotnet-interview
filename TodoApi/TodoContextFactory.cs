namespace TodoApi
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System.IO;

    public class TodoContextFactory : IDesignTimeDbContextFactory<TodoContext>
    {
        public TodoContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // busca appsettings.json en el cwd
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
            var connectionString = configuration.GetConnectionString("TodoContext");

            optionsBuilder.UseSqlServer(connectionString);

            return new TodoContext(optionsBuilder.Options);
        }
    }

}
