using System.Data.Entity;
using CTD.Core.Entities;
using CTD.Data.GenericRepository;

namespace CTD.Data.Context
{
    public class DatabaseContext : DbContext
    {
        private readonly DatabaseContext _context;
        private DbContextTransaction transaction;

        public DatabaseContext() : base("name=CtdEntities")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<CadSokBulv> CadSokBulvlar { get; set; }
        public virtual DbSet<Il> Iller { get; set; }
        public virtual DbSet<Ilce> Ilceler { get; set; }
        public virtual DbSet<IslemTurleri> IslemTurleri { get; set; }
        public virtual DbSet<Kullanici> Kullanicilar { get; set; }
        public virtual DbSet<Mahalle> Mahalleler { get; set; }
        public virtual DbSet<Makbuz> Makbuzlar { get; set; }
        public virtual DbSet<MakbuzDetay> MakbuzDetaylari { get; set; }
        public virtual DbSet<MakbuzDetayLog> MakbuzDetayLoglari { get; set; }
        public virtual DbSet<MakbuzDokum> MakbuzDokumleri { get; set; }
        public virtual DbSet<MakbuzDokumLog> MakbuzDokumLoglari { get; set; }
        public virtual DbSet<Meslekler> Meslekler { get; set; }
        public virtual DbSet<MeslekOdasi> MeslekOdalari { get; set; }
        public virtual DbSet<MeslekTerkNedeni> MeslekTerkNedenleri { get; set; }
        public virtual DbSet<Nace> Naceler { get; set; }
        public virtual DbSet<Sicil> Sicil { get; set; }
        public virtual DbSet<SicilMeslek> SicilMeslekler { get; set; }
        public virtual DbSet<SicilMeslekDegisiklik_Log> SicilMeslekDegisiklikLoglar { get; set; }
        public virtual DbSet<SicilMeslekLog> SicilMeslekLoglar { get; set; }
        public virtual DbSet<Sinif> Siniflar { get; set; }
        public virtual DbSet<TahsilatGruplari> TahsilatGruplari { get; set; }
        public virtual DbSet<TahsilatIslemleri> TahsilatIslemleri { get; set; }
        public virtual DbSet<TahsilatKalemleri> TahsilatKalemleri { get; set; }
        public virtual DbSet<TahsilatKalemleriFiyat> TahsilatKalemleriFiyat { get; set; }
        public virtual DbSet<TahsilatTurleri> TahsilatTurleri { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CadSokBulv>().ToTable("TBLCADSOKBULV", "dbo");
            modelBuilder.Entity<Il>().ToTable("TBLIL", "dbo");
            modelBuilder.Entity<Ilce>().ToTable("TBLILCE", "dbo");
            modelBuilder.Entity<IslemTurleri>().ToTable("TBLISLEMTURLERI", "dbo");
            modelBuilder.Entity<Kullanici>().ToTable("TBLUSERS", "dbo");
            modelBuilder.Entity<Mahalle>().ToTable("TBLMAHALLE", "dbo");
            modelBuilder.Entity<Makbuz>().ToTable("TBLMAKBUZ", "dbo");
            modelBuilder.Entity<Meslekler>().ToTable("TBLMESLEKLER", "dbo");
            modelBuilder.Entity<MeslekOdasi>().ToTable("MESLEKODASI", "dbo");
            modelBuilder.Entity<MeslekTerkNedeni>().ToTable("TBLMESLEKTERKNEDENI", "dbo");
            modelBuilder.Entity<Nace>().ToTable("TBLNACE", "dbo");
            modelBuilder.Entity<Sicil>().ToTable("SICIL", "dbo");
            modelBuilder.Entity<SicilMeslek>().ToTable("SICILMESLEK", "dbo");
            modelBuilder.Entity<SicilMeslekLog>().ToTable("SICILMESLEKLOG", "dbo");
            modelBuilder.Entity<SicilMeslekDegisiklik_Log>().ToTable("SICILMESLEKDEGISIKLIK_LOG", "dbo");
            modelBuilder.Entity<Sinif>().ToTable("TBLSINIFLAR", "dbo");
            modelBuilder.Entity<MakbuzDokum>().ToTable("TBLMAKBUZDOKUM", "dbo");
            modelBuilder.Entity<MakbuzDokumLog>().ToTable("TBLMAKBUZDOKUMLOG", "dbo");
            modelBuilder.Entity<MakbuzDetay>().ToTable("TBLMAKBUZDETAY", "dbo");
            modelBuilder.Entity<MakbuzDetayLog>().ToTable("TBLMAKBUZDETAYLOG", "dbo");
            modelBuilder.Entity<TahsilatIslemleri>().ToTable("TBLTAHSILATISLEMLERI", "dbo");
            modelBuilder.Entity<TahsilatGruplari>().ToTable("TBLTAHSILATGRUPLARI", "dbo");
            modelBuilder.Entity<TahsilatKalemleri>().ToTable("TBLTAHSILATKALEMLERI", "dbo");
            modelBuilder.Entity<TahsilatKalemleriFiyat>().ToTable("TBLTAHSILATKALEMLERIFIYAT", "dbo");
            modelBuilder.Entity<TahsilatTurleri>().ToTable("TBLTAHSILATTURLERI", "dbo");
            base.OnModelCreating(modelBuilder);
        }

        public void BeginTransaction()
        {
            transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }
    }
}