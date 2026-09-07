using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using PrezentacioniSloj.ViewModels;

namespace PrezentacioniSloj.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _apiAdresa = "http://localhost:44376/api/korisnik/login";

        private static readonly HttpClient _client = KreirajHttpClient();

        private static HttpClient KreirajHttpClient()
        {
            var handler = new HttpClientHandler();

#if DEBUG
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif

            return new HttpClient(handler);
        }

        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string jsonSadrzaj = JsonConvert.SerializeObject(model);
            HttpContent sadrzaj = new StringContent(jsonSadrzaj, Encoding.UTF8, "application/json");

            HttpResponseMessage odgovor = await _client.PostAsync(_apiAdresa, sadrzaj);

            if (odgovor.IsSuccessStatusCode)
            {
                Session["KorisnickoIme"] = model.KorisnickoIme;
                return RedirectToAction("Index", "ZahtevZaIzmenom");
            }

            ModelState.AddModelError("", "Неисправно корисничко име или лозинка.");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}