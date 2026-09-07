using System.Data.Entity;
using SlojPodataka.Entities;

namespace SlojPodataka.Context
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(string konekcioniString) : base(konekcioniString)
        {
            Database.SetInitializer<AppDbContext>(null);
            
            Configuration.LazyLoadingEnabled = false;
        }

       
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Klijent> Klijenti { get; set; }
        public DbSet<ZahtevZaIzmenom> ZahteviZaIzmenom { get; set; }
        public DbSet<StavkaZahteva> StavkeZahteva { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Klijent>().ToTable("Klijent");
            modelBuilder.Entity<ZahtevZaIzmenom>().ToTable("ZahtevZaIzmenom");
            modelBuilder.Entity<StavkaZahteva>().ToTable("StavkaZahteva");

            
            modelBuilder.Entity<StavkaZahteva>()
                .HasRequired(s => s.ZahtevZaIzmenom)
                .WithMany(z => z.Stavke)
                .HasForeignKey(s => s.ZahtevId);

            base.OnModelCreating(modelBuilder);
        }
    }
}