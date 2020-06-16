using CareStream.Models;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CareStream.Scheduler
{
    public class CareStreamContextFactory : IDesignTimeDbContextFactory<CareStreamContext>
    {
        public CareStreamContextFactory()
        {
            
        }
        
        public  CareStreamContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CareStreamContext>();
            builder.UseSqlServer(CareStreamConst.CareStreamConnectionString, x => x.MigrationsAssembly(typeof(CareStreamContext).GetTypeInfo().Assembly.GetName().Name));
            return new CareStreamContext(builder.Options);
        }
    }
}
