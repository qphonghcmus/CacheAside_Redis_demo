using CacheAside_Redis_demo.Models;
using Microsoft.EntityFrameworkCore;

namespace CacheAside_Redis_demo
{
    public class CountryContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public CountryContext(DbContextOptions dbContextOptions):base(dbContextOptions)
        {

        }
    }
}
