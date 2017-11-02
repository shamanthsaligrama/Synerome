using Microsoft.EntityFrameworkCore;
using SyneromeServices.Domain;

namespace SyneromeServices.Data
{
    public class SyneromeServicesContext : DbContext
    {
        public SyneromeServicesContext(DbContextOptions<SyneromeServicesContext> options): base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Nutritionists> Nutritionists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server = (localdb)\\mssqllocaldb; Database=SyneromeDB; Trusted_Connection = True;");
        }                    

    }
}
