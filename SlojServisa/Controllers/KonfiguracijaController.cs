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
                // Putanja do fajla u App_Data folderu
                string putanjaFajla = HostingEnvironment.MapPath("~/App_Data/poslovna_pravila.json");

                if (!File.Exists(putanjaFajla))
                {
                    return NotFound(); // Ako fajl ne postoji
                }

                // Čitamo ceo sadržaj fajla kao tekst
                string jsonSadrzaj = File.ReadAllText(putanjaFajla);

                // Parsiramo JSON pomoću Newtonsoft.Json (JObject)
                JObject jsonObj = JObject.Parse(jsonSadrzaj);

                // Izvlačimo vrednost X prema našoj hijerarhiji
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