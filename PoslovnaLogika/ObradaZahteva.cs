using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using SlojPodataka.Repozitorijumi;

namespace PoslovnaLogika
{
    public class ObradaZahteva
    {
        private readonly string _konfiguracijaFajl;
        private readonly StavkaZahtevaRepo _stavkaRepo;

        public ObradaZahteva(string konfiguracijaFajl, string konekcioniString)
            
        {
            _konfiguracijaFajl = konfiguracijaFajl;
            _stavkaRepo = new StavkaZahtevaRepo(konekcioniString);
        }

        
        public int UcitajKonfiguraciju()
        {
            if (!File.Exists(_konfiguracijaFajl))
            {
                return 40; 
            }

            string jsonSadrzaj = File.ReadAllText(_konfiguracijaFajl);
            JObject jsonObj = JObject.Parse(jsonSadrzaj);

            return jsonObj["pravila_odobrenja"]["limit_sati_za_odobrenje"]?.Value<int>() ?? 40;
        }

        
        public bool ProbijaLimitBezPotvrde(int zahtevId, decimal ukupnoSati, string status, bool potvrdaMenadzera)
        {
            int limitSati = UcitajKonfiguraciju();

            
            if (ukupnoSati == 0 && zahtevId > 0)
            {
                var stavke = _stavkaRepo.DajPoIdZahteva(zahtevId);
                ukupnoSati = stavke?.Sum(s => s.ProcenjeniSati) ?? 0;

            }

            if ((status == "У реализацији" || status == "U realizaciji") && ukupnoSati > limitSati && !potvrdaMenadzera)
            {
                return true;
            }

            return false;
        }

        public void PripremiZahtevZaStampu(int zahtevId)
        {
            
            throw new System.NotImplementedException();
        }
    }
}