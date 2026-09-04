using SlojPodataka.Entities;
using SlojPodataka.Repozitorijumi;
using System.Configuration;
using System.Web.Http;

namespace SlojServisa.Controllers
{
    [RoutePrefix("api/korisnik")]
    public class KorisnikController : ApiController
    {
        private readonly KorisnikRepo _korisnikRepo;

        public KorisnikController()
        {
            // Čitamo konekcioni string iz Web.config fajla
            string konekcioniString = ConfigurationManager.ConnectionStrings["UpravljanjeZahtevimaDB"].ConnectionString;

            // Inicijalizujemo repozitorijum
            _korisnikRepo = new KorisnikRepo(konekcioniString);
        }

        // 1. Prijava (Login) - Autentifikacija
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Prijava([FromBody] Korisnik podaciZaPrijavu)
        {
            // Provera da li je prosleđen objekat i da li su popunjena obavezna polja (KorisnickoIme umesto Email)
            if (podaciZaPrijavu == null || string.IsNullOrEmpty(podaciZaPrijavu.KorisnickoIme) || string.IsNullOrEmpty(podaciZaPrijavu.Lozinka))
            {
                return BadRequest("Nedostaju podaci za prijavu."); // HTTP 400
            }

            // Pozivamo optimizovanu metodu koja direktno u bazi vrši filtriranje preko procedure spKorisnik_Prijava
            var korisnik = _korisnikRepo.DajPoKorisnickomImenuILozinci(podaciZaPrijavu.KorisnickoIme, podaciZaPrijavu.Lozinka);

            if (korisnik == null)
            {
                return Unauthorized(); // HTTP 401 - Pogrešni kredencijali
            }

            return Ok(korisnik); // HTTP 200 - Uspešna prijava, vraćamo podatke korisnika
        }

        // 2. Čitanje svih korisnika (Read All)
        [HttpGet]
        [Route("")]
        public IHttpActionResult DajSve()
        {
            // Pozivamo repozitorijum da dohvati listu
            var korisnici = _korisnikRepo.DajSve();

            // Vraćamo HTTP 200 OK i listu korisnika (koja se automatski serijalizuje u JSON)
            return Ok(korisnici);
        }

        // 3. Čitanje jednog korisnika po ID-u (Read by ID)
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult DajPoId(int id)
        {
            var korisnik = _korisnikRepo.DajPoId(id);
            if (korisnik == null)
            {
                return NotFound(); // HTTP 404
            }
            return Ok(korisnik); // HTTP 200
        }

        // 4. Kreiranje novog korisnika (Create)
        [HttpPost]
        [Route("")]
        public IHttpActionResult Dodaj([FromBody] Korisnik entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            _korisnikRepo.Dodaj(entitet);
            return Ok(entitet); // HTTP 200
        }

        // 5. Ažuriranje postojećeg korisnika (Update)
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Izmeni(int id, [FromBody] Korisnik entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            entitet.Id = id; // Osiguravamo da se ažurira tačan korisnik
            _korisnikRepo.Izmeni(entitet);

            return Ok(); // HTTP 200
        }

        // 6. Brisanje korisnika (Delete)
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Obrisi(int id)
        {
            var postojeci = _korisnikRepo.DajPoId(id);
            if (postojeci == null)
            {
                return NotFound(); // HTTP 404
            }

            _korisnikRepo.Obrisi(id);
            return Ok(); // HTTP 200
        }
    }
}