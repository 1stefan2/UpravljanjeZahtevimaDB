using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SlojPodataka.Context;
using SlojPodataka.Entities;

namespace SlojPodataka.Repozitorijumi
{
    public class ZahtevZaIzmenomRepo : OsnovniRepozitorijum<ZahtevZaIzmenom>
    {
        public ZahtevZaIzmenomRepo(string connectionString) : base(connectionString)
        {
        }

        public override List<ZahtevZaIzmenom> DajSve()
        {
            using (var context = new AppDbContext(_konekcioniString))
            {
                return context.ZahteviZaIzmenom
                              .Include(z => z.Klijent)
                              .Include(z => z.Korisnik)
                              .ToList();
            }
        }

        public override ZahtevZaIzmenom DajPoId(int id)
        {
            using (var context = new AppDbContext(_konekcioniString))
            {
                return context.ZahteviZaIzmenom
                              .Include(z => z.Klijent)
                              .Include(z => z.Korisnik)
                              .Include(z => z.Stavke) 
                              .FirstOrDefault(z => z.Id == id);
            }
        }

        public override void Dodaj(ZahtevZaIzmenom entitet)
        {
            using (var context = new AppDbContext(_konekcioniString))
            {
                using (var transakcija = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.ZahteviZaIzmenom.Add(entitet);
                        context.SaveChanges();
                        transakcija.Commit();
                    }
                    catch
                    {
                        transakcija.Rollback();
                        throw;
                    }
                }
            }
        }



        public override void Izmeni(ZahtevZaIzmenom entity)
        {
            using (var context = new AppDbContext(_konekcioniString))
            {
                // 1. Učitavamo postojeći entitet iz baze zajedno sa njegovim stavkama
                var existingEntity = context.ZahteviZaIzmenom
                    .Include(z => z.Stavke)
                    .FirstOrDefault(z => z.Id == entity.Id);

                if (existingEntity != null)
                {
                    // 2. Ažuriramo osnovna polja (zaglavlje / Master)
                    context.Entry(existingEntity).CurrentValues.SetValues(entity);

                    // 3. Uklanjamo sve stare stavke iz baze
                    context.StavkeZahteva.RemoveRange(existingEntity.Stavke);

                    // 4. Dodajemo nove stavke koje su stigle sa forme
                    if (entity.Stavke != null)
                    {
                        foreach (var stavka in entity.Stavke)
                        {
                            stavka.Id = 0; // Resetujemo ID na 0 da bi EF znao da su ovo novi unosi za insert
                            existingEntity.Stavke.Add(stavka);
                        }
                    }

                    // 5. Čuvamo sve promene u jednoj transakciji
                    context.SaveChanges();
                }
            }
        }


        public override void Obrisi(int id)
        {
            using (var context = new AppDbContext(_konekcioniString))
            {
                var entity = context.ZahteviZaIzmenom
                    .Include(z => z.Stavke)
                    .FirstOrDefault(z => z.Id == id);

                if (entity != null)
                {
                    // Prvo brišemo stavke pa onda zahtev
                    context.StavkeZahteva.RemoveRange(entity.Stavke);
                    context.ZahteviZaIzmenom.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        public bool PostojiBrojZahteva(string brojZahteva)
        {
            using (var context = new AppDbContext(_konekcioniString))
            {
                return context.ZahteviZaIzmenom.Any(z => z.BrojZahteva == brojZahteva);
            }
        }

    }
}