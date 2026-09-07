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
            
            string konekcioniString = ConfigurationManager.ConnectionStrings["UpravljanjeZahtevimaDB"].ConnectionString;

            
            _klijentRepo = new KlijentRepo(konekcioniString);
        }

        
        [HttpGet]
        [Route("")]
        public IHttpActionResult DajSve()
        {
            var klijenti = _klijentRepo.DajSve();
            return Ok(klijenti); 
        }

        
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult DajPoId(int id)
        {
            var klijent = _klijentRepo.DajPoId(id);

            if (klijent == null)
            {
                return NotFound(); 
            }

            return Ok(klijent); 
        }

        
        [HttpPost]
        [Route("")]
        public IHttpActionResult Dodaj([FromBody] Klijent entitet)
        {
            if (entitet == null)
            {
                return BadRequest("Prosleđen je prazan objekat.");
            }

            _klijentRepo.Dodaj(entitet);
            return Ok(entitet); 
        }

        
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

            return Ok(); 
        }

        
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
            return Ok(); 
        }
    }
}