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
            
            string konekcioniString = ConfigurationManager.ConnectionStrings["UpravljanjeZahtevimaDB"].ConnectionString;

           
            _korisnikRepo = new KorisnikRepo(konekcioniString);
        }

        
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Prijava([FromBody] Korisnik podaciZaPrijavu)
        {
            if (podaciZaPrijavu == null || string.IsNullOrEmpty(podaciZaPrijavu.KorisnickoIme) || string.IsNullOrEmpty(podaciZaPrijavu.Lozinka))
            {
                return BadRequest("Nedostaju podaci za prijavu."); 
            }

            var korisnik = _korisnikRepo.DajPoKorisnickomImenuILozinci(podaciZaPrijavu.KorisnickoIme, podaciZaPrijavu.Lozinka);

            if (korisnik == null)
            {
                return Unauthorized(); 
            }

            return Ok(korisnik); 
        }

        
        [HttpGet]
        [Route("")]
        public IHttpActionResult DajSve()
        {
           
            var korisnici = _korisnikRepo.DajSve();

            
            return Ok(korisnici);
        }

        
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult DajPoId(int id)
        {
            var korisnik = _korisnikRepo.DajPoId(id);
            if (korisnik == null)
            {
                return NotFound(); 
            }
            return Ok(korisnik); 
        }

        
        [HttpPost]
        [Route("")]
        public IHttpActionResult Dodaj([FromBody] Korisnik entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            _korisnikRepo.Dodaj(entitet);
            return Ok(entitet); 
        }

        
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Izmeni(int id, [FromBody] Korisnik entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            entitet.Id = id; 
            _korisnikRepo.Izmeni(entitet);

            return Ok(); 
        }

        
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Obrisi(int id)
        {
            var postojeci = _korisnikRepo.DajPoId(id);
            if (postojeci == null)
            {
                return NotFound(); 
            }

            _korisnikRepo.Obrisi(id);
            return Ok(); 
        }
    }
}