using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace SlojServisa.Controllers
{
    [RoutePrefix("api/konfiguracija")]
    public class KonfiguracijaController : ApiController
    {
        [HttpGet]
        [Route("limit-sati")]
        public IHttpActionResult DajLimitSati()
        {
            try
            {
                
                string putanjaFajla = HostingEnvironment.MapPath("~/App_Data/poslovna_pravila.json");

                if (!File.Exists(putanjaFajla))
                {
                    return NotFound(); 
                }

                
                string jsonSadrzaj = File.ReadAllText(putanjaFajla);

                
                JObject jsonObj = JObject.Parse(jsonSadrzaj);

                
                int limitSati = jsonObj["pravila_odobrenja"]["limit_sati_za_odobrenje"]?.Value<int>() ?? 40;

                return Ok(new { limit_sati_za_odobrenje = limitSati });
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}