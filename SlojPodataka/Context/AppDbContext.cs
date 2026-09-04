using System.Data.Entity;
using SlojPodataka.Entities;

namespace SlojPodataka.Context
{
    public class AppDbContext : DbContext
    {
        // Konstruktor koji prima konekcioni string i prosleđuje ga baznoj DbContext klasi
        public AppDbContext(string konekcioniString) : base(konekcioniString)
        {
            Database.SetInitializer<AppDbContext>(null);
            // Isključujemo lazy loading radi bolje kontrole i performansi nad relacijama
            Configuration.LazyLoadingEnabled = false;
        }

        // DbSet properties za sve 4 tabele u bazi
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Klijent> Klijenti { get; set; }
        public DbSet<ZahtevZaIzmenom> ZahteviZaIzmenom { get; set; }
        public DbSet<StavkaZahteva> StavkeZahteva { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Eksplicitno mapiramo entitete na tačne nazive tabela u bazi bez sufiksa "s"
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Klijent>().ToTable("Klijent");
            modelBuilder.Entity<ZahtevZaIzmenom>().ToTable("ZahtevZaIzmenom");
            modelBuilder.Entity<StavkaZahteva>().ToTable("StavkaZahteva");

            // Mapiranje FK veze
            modelBuilder.Entity<StavkaZahteva>()
                .HasRequired(s => s.ZahtevZaIzmenom)
                .WithMany(z => z.Stavke)
                .HasForeignKey(s => s.ZahtevId);

            base.OnModelCreating(modelBuilder);
        }
    }
}