using Microsoft.EntityFrameworkCore.Design;

namespace HeldenServer.Database
{
    public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {

        public DatabaseContext CreateDbContext(string[] args)
        {
            return new DatabaseContext();
        }

    }
}
