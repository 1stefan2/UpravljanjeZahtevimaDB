using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using SlojPodataka.Entities;
using SlojPodataka.Repozitorijumi;
using System.Configuration;
using PoslovnaLogika;

namespace SlojServisa.Controllers
{
    [RoutePrefix("api/zahtev")]
    public class ZahtevZaIzmenomController : ApiController
    {
        private readonly ZahtevZaIzmenomRepo _zahtevRepo;
        private readonly StavkaZahtevaRepo _stavkaRepo;

        public ZahtevZaIzmenomController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["UpravljanjeZahtevimaDB"].ConnectionString;
            _zahtevRepo = new ZahtevZaIzmenomRepo(connectionString);
            _stavkaRepo = new StavkaZahtevaRepo(connectionString);
        }

        // Pomoćna metoda za instanciranje poslovne logike
        private ObradaZahteva InicijalizujObraduZahteva()
        {
            string putanjaJson = HostingEnvironment.MapPath("~/App_Data/poslovna_pravila.json");
            string connString = ConfigurationManager.ConnectionStrings["UpravljanjeZahtevimaDB"].ConnectionString;
            return new ObradaZahteva(putanjaJson, connString);
        }

        // 1. Čitanje svih zahteva (sa opcionim filtriranjem po statusu i broju zahteva)
        [HttpGet]
        [Route("")]
        public IHttpActionResult DajSve(string status = null, string brojZahteva = null)
        {
            var zahtevi = _zahtevRepo.DajSve();

            if (!string.IsNullOrEmpty(status))
            {
                zahtevi = zahtevi.Where(z => z.Status == status).ToList();
            }

            if (!string.IsNullOrEmpty(brojZahteva))
            {
                zahtevi = zahtevi.Where(z => z.BrojZahteva.Contains(brojZahteva)).ToList();
            }

            return Ok(zahtevi);
        }

        // 2. Čitanje jednog zahteva po ID-u
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult DajPoId(int id)
        {
            var zahtev = _zahtevRepo.DajPoId(id);
            if (zahtev == null)
            {
                return NotFound();
            }
            return Ok(zahtev);
        }

        // 3. Kreiranje novog zahteva
        [HttpPost]
        [Route("")]
        public IHttpActionResult Dodaj([FromBody] ZahtevZaIzmenom entitet)
        {
            if (entitet == null)
                return BadRequest("Prosleđen je prazan objekat.");

            if (_zahtevRepo.PostojiBrojZahteva(entitet.BrojZahteva))
            {
                return BadRequest($"Zahtev sa brojem '{entitet.BrojZahteva}' već postoji u sistemu.");
            }

            var obrada = InicijalizujObraduZahteva();
            decimal ukupnoSati = entitet.Stavke?.Sum(s => s.ProcenjeniSati) ?? 0;

            if (obrada.ProbijaLimitBezPotvrde(entitet.Id, ukupnoSati, entitet.Status, entitet.PotvrdaMenadzera))
            {
                int limit = obrada.UcitajKonfiguraciju();
                return BadRequest($"Poslovno pravilo: Zahtevi čija procena prelazi {limit} sati moraju imati potvrdu menadžera pre realizacije.");
            }

            _zahtevRepo.Dodaj(entitet);
            return Ok(entitet);
        }

        // 4. Ažuriranje postojećeg zahteva (Standardni update)
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Izmeni(int id, [FromBody] ZahtevZaIzmenom entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            var obrada = InicijalizujObraduZahteva();
            decimal ukupnoSati = entitet.Stavke?.Sum(s => s.ProcenjeniSati) ?? 0;

            if (obrada.ProbijaLimitBezPotvrde(id, ukupnoSati, entitet.Status, entitet.PotvrdaMenadzera))
            {
                int limit = obrada.UcitajKonfiguraciju();
                return BadRequest($"Poslovno pravilo: Zahtevi čija procena prelazi {limit} sati moraju imati potvrdu menadžera pre realizacije.");
            }

            entitet.Id = id;
            _zahtevRepo.Izmeni(entitet);
            return Ok();
        }

        // 5. Promena statusa i validacija pravila
        [HttpPut]
        [Route("{id}/status")]
        public IHttpActionResult PromeniStatus(int id, [FromBody] ZahtevZaIzmenom izmenaPodataka)
        {
            if (izmenaPodataka == null || string.IsNullOrEmpty(izmenaPodataka.Status))
            {
                return BadRequest("Neispravni podaci za promenu statusa.");
            }

            var postojeciZahtev = _zahtevRepo.DajPoId(id);
            if (postojeciZahtev == null)
            {
                return NotFound();
            }

            var obrada = InicijalizujObraduZahteva();
            bool imaPotvrdu = izmenaPodataka.PotvrdaMenadzera || postojeciZahtev.PotvrdaMenadzera;

            if (obrada.ProbijaLimitBezPotvrde(id, 0, izmenaPodataka.Status, imaPotvrdu))
            {
                int limit = obrada.UcitajKonfiguraciju();
                return BadRequest($"Odbijeno: Zahtev prelazi dozvoljeni limit od {limit}h. Neophodna je potvrda menadžera pre realizacije.");
            }

            postojeciZahtev.Status = izmenaPodataka.Status;
            postojeciZahtev.PotvrdaMenadzera = izmenaPodataka.PotvrdaMenadzera;

            _zahtevRepo.Izmeni(postojeciZahtev);

            return Ok(postojeciZahtev);
        }

        // 6. Brisanje zahteva
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Obrisi(int id)
        {
            var postojeci = _zahtevRepo.DajPoId(id);
            if (postojeci == null)
            {
                return NotFound();
            }

            _zahtevRepo.Obrisi(id);
            return Ok();
        }
    }
}