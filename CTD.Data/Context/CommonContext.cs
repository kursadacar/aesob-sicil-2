using System.Data.Entity;

namespace CTD.Data.Context
{
    public class CommonContext : DbContext
    {
        public CommonContext() : base("name=CtdEntities")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}