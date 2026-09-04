using SlojPodataka.Entities;
using SlojPodataka.Repozitorijumi;
using System.Configuration;
using System.Web.Http;

namespace SlojServisa.Controllers
{
    [RoutePrefix("api/klijent")]
    public class KlijentController : ApiController
    {
        private readonly KlijentRepo _klijentRepo;

        public KlijentController()
        {
            // Čitamo konekcioni string iz Web.config fajla
            string konekcioniString = ConfigurationManager.ConnectionStrings["UpravljanjeZahtevimaDB"].ConnectionString;

            // Inicijalizujemo KlijentRepo (koji koristi naš DBUtils wraper)
            _klijentRepo = new KlijentRepo(konekcioniString);
        }

        // 1. Čitanje svih klijenata (Read All)
        [HttpGet]
        [Route("")]
        public IHttpActionResult DajSve()
        {
            var klijenti = _klijentRepo.DajSve();
            return Ok(klijenti); // HTTP 200
        }

        // 2. Čitanje jednog klijenta po ID-u (Read by ID)
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult DajPoId(int id)
        {
            var klijent = _klijentRepo.DajPoId(id);

            if (klijent == null)
            {
                return NotFound(); // HTTP 404
            }

            return Ok(klijent); // HTTP 200
        }

        // 3. Kreiranje novog klijenta (Create)
        [HttpPost]
        [Route("")]
        public IHttpActionResult Dodaj([FromBody] Klijent entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            _klijentRepo.Dodaj(entitet);
            return Ok(entitet); // HTTP 200
        }

        // 4. Ažuriranje postojećeg klijenta (Update)
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Izmeni(int id, [FromBody] Klijent entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            entitet.Id = id;
            _klijentRepo.Izmeni(entitet);

            return Ok(); // HTTP 200
        }

        // 5. Brisanje klijenta (Delete)
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Obrisi(int id)
        {
            var postojeci = _klijentRepo.DajPoId(id);
            if (postojeci == null)
            {
                return NotFound();
            }

            _klijentRepo.Obrisi(id);
            return Ok(); // HTTP 200
        }
    }
}