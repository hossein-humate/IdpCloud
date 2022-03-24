using IdpCloud.DataProvider.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdpCloud.Tests.InMemory
{
    public class TestBase 
    {
        protected DbContextOptions<EfCoreContext> GetMockContextOptions()
        {
            DbContextOptions<EfCoreContext> options;
            var builder = new DbContextOptionsBuilder<EfCoreContext>();
            builder.UseInMemoryDatabase("InMemoryDB-" + Guid.NewGuid().ToString());
            options = builder.Options;
            
            return options;
        }
        
    }
}
