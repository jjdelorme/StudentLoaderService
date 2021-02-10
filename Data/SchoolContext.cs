using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace Cymbal.Data
{
    public class SchoolContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
