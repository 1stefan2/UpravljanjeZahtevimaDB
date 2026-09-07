using Newtonsoft.Json;
using PrezentacioniSloj.ViewModels;
using SlojPodataka.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PrezentacioniSloj.Controllers
{
    public class ZahtevZaIzmenomController : Controller
    {
        private readonly string _apiAdresa = "http://localhost:44376/api/zahtev";
        private readonly string _apiKlijentUrl = "http://localhost:44376/api/klijent";
        private readonly string _apiKorisnikUrl = "http://localhost:44376/api/korisnik";


        [HttpGet]
        public async Task<ActionResult> Index(string statusFilter, string brojZahtevaFilter)
        {
            List<ZahtevZaIzmenomViewModel> modeli = new List<ZahtevZaIzmenomViewModel>();

            using (HttpClient client = new HttpClient())
            {
                string url = _apiAdresa;
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    url += $"?status={System.Net.WebUtility.UrlEncode(statusFilter)}";
                }

                if (!string.IsNullOrEmpty(brojZahtevaFilter))
                {
                    url += (url.Contains("?") ? "&" : "?") + $"brojZahteva={System.Net.WebUtility.UrlEncode(brojZahtevaFilter)}";
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var entiteti = JsonConvert.DeserializeObject<List<ZahtevZaIzmenom>>(jsonResponse);

                    if (entiteti != null)
                    {
                        modeli = entiteti.Select(e => new ZahtevZaIzmenomViewModel
                        {
                            Id = e.Id,
                            BrojZahteva = e.BrojZahteva,
                            DatumPodnosenja = e.DatumPodnosenja,
                            NazivProjekta = e.NazivProjekta,
                            NazivKlijenta = e.Klijent != null ? e.Klijent.NazivFirme : "",
                            Status = e.Status
                        }).ToList();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Došlo je do greške prilikom učitavanja zahteva sa servera.";
                }
            }

            return View(modeli);
        }


        
        [HttpGet]
        public async Task<ActionResult> Kreiraj()
        {
            var model = new KreiranjeZahtevaViewModel
            {
                Zahtev = new ZahtevZaIzmenomViewModel
                {
                    DatumPodnosenja = DateTime.Today
                }
            };

            await PopuniPadajuceListe(model);

            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Kreiraj(KreiranjeZahtevaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopuniPadajuceListe(model);
                return View(model);
            }

            try
            {
                var zahtev = new ZahtevZaIzmenom
                {
                    BrojZahteva = model.Zahtev.BrojZahteva,
                    DatumPodnosenja = model.Zahtev.DatumPodnosenja,
                    NazivProjekta = model.Zahtev.NazivProjekta,
                    ZahtevPodneo = model.Zahtev.ZahtevPodneo,
                    Status = model.Zahtev.Status,
                    DetaljanOpis = model.Zahtev.DetaljanOpis,
                    PoslovniRazlog = model.Zahtev.PoslovniRazlog,
                    UticajNaRok = model.Zahtev.UticajNaRok,
                    NoviDatumIsporuke = model.Zahtev.NoviDatumIsporuke,
                    PotvrdaMenadzera = model.Zahtev.PotvrdaMenadzera,
                    KlijentId = model.IzabranKlijentId,
                    KorisnikId = model.IzabranKorisnikId,

                    Stavke = model.Stavke != null
                    ? model.Stavke.Select(s => new StavkaZahteva
                    {
                         FazaVrstaPosla = s.FazaIliVrstaPosla,
                         ProcenjeniSati = s.ProcenjenoSati,
                        OznakaStavke = model.Stavke.IndexOf(s) + 1
                    }).ToList()
                    : new List<StavkaZahteva>()

                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:44376/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string jsonPayload = JsonConvert.SerializeObject(zahtev);
                    HttpContent content = new StringContent(
                        jsonPayload,
                        System.Text.Encoding.UTF8,
                        "application/json");

                    HttpResponseMessage response = await client.PostAsync("api/zahtev", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        string porukaZaKorisnika = "Došlo je do greške prilikom obrade zahteva na serveru.";

                        try
                        {
                            
                            var jsonError = Newtonsoft.Json.Linq.JObject.Parse(errorContent);

                            
                            if (jsonError["Message"] != null)
                            {
                                porukaZaKorisnika = jsonError["Message"].ToString();
                            }
                        }
                        catch
                        {
                            
                            porukaZaKorisnika = errorContent;
                        }

                        ModelState.AddModelError(string.Empty, porukaZaKorisnika);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Došlo je do izuzetka prilikom komunikacije sa API-jem: " + ex.Message);
            }

            await PopuniPadajuceListe(model);
            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> Detalji(int id)
        {
            KreiranjeZahtevaViewModel model = new KreiranjeZahtevaViewModel();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:44376/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/zahtev/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var entitet = JsonConvert.DeserializeObject<ZahtevZaIzmenom>(jsonResponse);

                    if (entitet == null)
                    {
                        return HttpNotFound();
                    }

                    model.Zahtev = new ZahtevZaIzmenomViewModel
                    {
                        Id = entitet.Id,
                        BrojZahteva = entitet.BrojZahteva,
                        DatumPodnosenja = entitet.DatumPodnosenja,
                        KlijentId = entitet.KlijentId,
                        KorisnikId = entitet.KorisnikId,
                        NazivProjekta = entitet.NazivProjekta,
                        ZahtevPodneo = entitet.ZahtevPodneo,
                        Status = entitet.Status,
                        DetaljanOpis = entitet.DetaljanOpis,
                        PoslovniRazlog = entitet.PoslovniRazlog,
                        UticajNaRok = entitet.UticajNaRok,
                        NoviDatumIsporuke = entitet.NoviDatumIsporuke,
                        PotvrdaMenadzera = entitet.PotvrdaMenadzera
                    };

                    model.Stavke = entitet.Stavke != null
                        ? entitet.Stavke.Select(s => new StavkaZahtevaViewModel
                        {
                            Id = s.Id,
                            FazaIliVrstaPosla = s.FazaVrstaPosla,
                            ProcenjenoSati = s.ProcenjeniSati
                        }).ToList()
                        : new List<StavkaZahtevaViewModel>();

                    model.IzabranKlijentId = entitet.KlijentId;
                    model.IzabranKorisnikId = entitet.KorisnikId;
                }
                else
                {
                    return HttpNotFound("Traženi zahtev za izmenom nije pronađen.");
                }
            }

            return View(model);
        }

        
        [HttpGet]
        public async Task<ActionResult> Izmeni(int id)
        {
            KreiranjeZahtevaViewModel model = new KreiranjeZahtevaViewModel();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:44376/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/zahtev/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var entitet = JsonConvert.DeserializeObject<ZahtevZaIzmenom>(jsonResponse);

                    if (entitet == null)
                    {
                        return HttpNotFound();
                    }

                    model.Zahtev = new ZahtevZaIzmenomViewModel
                    {
                        Id = entitet.Id,
                        BrojZahteva = entitet.BrojZahteva,
                        DatumPodnosenja = entitet.DatumPodnosenja,
                        KlijentId = entitet.KlijentId,
                        KorisnikId = entitet.KorisnikId,
                        NazivProjekta = entitet.NazivProjekta,
                        ZahtevPodneo = entitet.ZahtevPodneo,
                        Status = entitet.Status,
                        DetaljanOpis = entitet.DetaljanOpis,
                        PoslovniRazlog = entitet.PoslovniRazlog,
                        UticajNaRok = entitet.UticajNaRok,
                        NoviDatumIsporuke = entitet.NoviDatumIsporuke,
                        PotvrdaMenadzera = entitet.PotvrdaMenadzera
                    };

                    
                    model.Stavke = entitet.Stavke != null
                        ? entitet.Stavke.Select(s => new StavkaZahtevaViewModel
                        {
                            Id = s.Id,
                            FazaIliVrstaPosla = s.FazaVrstaPosla,
                            ProcenjenoSati = s.ProcenjeniSati
                        }).ToList()
                        : new List<StavkaZahtevaViewModel>();


                    model.IzabranKlijentId = entitet.KlijentId;
                    model.IzabranKorisnikId = entitet.KorisnikId;
                }
                else
                {
                    return HttpNotFound("Traženi zahtev za izmenom nije pronađen.");
                }
            }

            await PopuniPadajuceListe(model);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Izmeni(KreiranjeZahtevaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopuniPadajuceListe(model);
                return View(model);
            }

            try
            {
                var zahtev = new ZahtevZaIzmenom
                {
                    Id = model.Zahtev.Id,
                    BrojZahteva = model.Zahtev.BrojZahteva,
                    DatumPodnosenja = model.Zahtev.DatumPodnosenja,
                    NazivProjekta = model.Zahtev.NazivProjekta,
                    ZahtevPodneo = model.Zahtev.ZahtevPodneo,
                    Status = model.Zahtev.Status,
                    DetaljanOpis = model.Zahtev.DetaljanOpis,
                    PoslovniRazlog = model.Zahtev.PoslovniRazlog,
                    UticajNaRok = model.Zahtev.UticajNaRok,
                    NoviDatumIsporuke = model.Zahtev.NoviDatumIsporuke,
                    PotvrdaMenadzera = model.Zahtev.PotvrdaMenadzera,
                    KlijentId = model.IzabranKlijentId,
                    KorisnikId = model.IzabranKorisnikId,


                    Stavke = model.Stavke != null
                    ? model.Stavke.Select(s => new StavkaZahteva
                    {
                     FazaVrstaPosla = s.FazaIliVrstaPosla,
                     ProcenjeniSati = s.ProcenjenoSati,
                     OznakaStavke = model.Stavke.IndexOf(s) + 1
                    }).ToList(): new List<StavkaZahteva>()
                };



                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:44376/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string jsonPayload = JsonConvert.SerializeObject(zahtev);
                    HttpContent content = new StringContent(
                        jsonPayload,
                        System.Text.Encoding.UTF8,
                        "application/json");

                    HttpResponseMessage response = await client.PutAsync($"api/zahtev/{model.Zahtev.Id}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        string porukaZaKorisnika = "Došlo je do greške prilikom obrade zahteva na serveru.";

                        try
                        {
                            
                            var jsonError = Newtonsoft.Json.Linq.JObject.Parse(errorContent);

                            
                            if (jsonError["Message"] != null)
                            {
                                porukaZaKorisnika = jsonError["Message"].ToString();
                            }
                        }
                        catch
                        {
                            
                            porukaZaKorisnika = errorContent;
                        }

                        ModelState.AddModelError(string.Empty, porukaZaKorisnika);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Došlo je do izuzetka prilikom komunikacije sa API-jem: " + ex.Message);
            }

            await PopuniPadajuceListe(model);
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Obrisi(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:44376/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"api/zahtev/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Greška prilikom brisanja zahteva.";
                    return RedirectToAction("Index");
                }
            }
        }

       
        [HttpGet]
        public async Task<ActionResult> Stampaj(int id)
        {
            KreiranjeZahtevaViewModel model = new KreiranjeZahtevaViewModel();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:44376/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/zahtev/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var entitet = JsonConvert.DeserializeObject<ZahtevZaIzmenom>(jsonResponse);

                    model.Zahtev = new ZahtevZaIzmenomViewModel
                    {
                        Id = entitet.Id,
                        BrojZahteva = entitet.BrojZahteva,
                        DatumPodnosenja = entitet.DatumPodnosenja,
                        NazivProjekta = entitet.NazivProjekta,
                        NazivKlijenta = entitet.Klijent != null ? entitet.Klijent.NazivFirme : "",
                        ZahtevPodneo = entitet.ZahtevPodneo,
                        Status = entitet.Status,
                        DetaljanOpis = entitet.DetaljanOpis,
                        PoslovniRazlog = entitet.PoslovniRazlog,
                        UticajNaRok = entitet.UticajNaRok,
                        NoviDatumIsporuke = entitet.NoviDatumIsporuke,
                        PotvrdaMenadzera = entitet.PotvrdaMenadzera
                    };

                    model.Stavke = entitet.Stavke != null
                        ? entitet.Stavke.Select(s => new StavkaZahtevaViewModel
                        {
                            Id = s.Id,
                            FazaIliVrstaPosla = s.FazaVrstaPosla,
                            ProcenjenoSati = s.ProcenjeniSati
                        }).ToList()
                        : new List<StavkaZahtevaViewModel>();
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> SpisakSvih()
        {
            
            return RedirectToAction("Index");
        }

        
        private async Task PopuniPadajuceListe(KreiranjeZahtevaViewModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:44376/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                
                HttpResponseMessage klijentiResponse = await client.GetAsync(_apiKlijentUrl);
                if (klijentiResponse.IsSuccessStatusCode)
                {
                    string jsonKlijenti = await klijentiResponse.Content.ReadAsStringAsync();
                    var klijentiList = JsonConvert.DeserializeObject<List<Klijent>>(jsonKlijenti)
                                       ?? new List<Klijent>();

                    model.ListaKlijenata = klijentiList.Select(k => new SelectListItem
                    {
                        Value = k.Id.ToString(),
                        Text = k.NazivFirme
                    }).ToList();
                }
                else
                {
                    model.ListaKlijenata = new List<SelectListItem>();
                }

                
                HttpResponseMessage korisniciResponse = await client.GetAsync(_apiKorisnikUrl);
                if (korisniciResponse.IsSuccessStatusCode)
                {
                    string jsonKorisnici = await korisniciResponse.Content.ReadAsStringAsync();
                    var korisniciList = JsonConvert.DeserializeObject<List<Korisnik>>(jsonKorisnici)
                                        ?? new List<Korisnik>();

                    model.ListaKorisnika = korisniciList.Select(ko => new SelectListItem
                    {
                        Value = ko.Id.ToString(),
                        Text = ko.KorisnickoIme
                    }).ToList();
                }
                else
                {
                    model.ListaKorisnika = new List<SelectListItem>();
                }
            }
        }
    }
}