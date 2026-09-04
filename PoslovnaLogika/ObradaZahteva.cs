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

        // 1. Čitanje limita iz JSON-a
        public int UcitajKonfiguraciju()
        {
            if (!File.Exists(_konfiguracijaFajl))
            {
                return 40; // Default vrednost ako fajl ne postoji
            }

            string jsonSadrzaj = File.ReadAllText(_konfiguracijaFajl);
            JObject jsonObj = JObject.Parse(jsonSadrzaj);

            return jsonObj["pravila_odobrenja"]["limit_sati_za_odobrenje"]?.Value<int>() ?? 40;
        }

        // 2. Provera poslovnog pravila (AKO-ONDA)
        public bool ProbijaLimitBezPotvrde(int zahtevId, decimal ukupnoSati, string status, bool potvrdaMenadzera)
        {
            int limitSati = UcitajKonfiguraciju();

            // Ako sate nismo već prosledili iz entiteta, čitamo ih iz baze preko Repozitorijuma
            if (ukupnoSati == 0 && zahtevId > 0)
            {
                var stavke = _stavkaRepo.DajPoIdZahteva(zahtevId);
                ukupnoSati = stavke?.Sum(s => s.ProcenjeniSati) ?? 0;
            }

            // AKO ide u realizaciju, ima više sati od limita I nema potvrdu -> pravilo je prekršeno
            if ((status == "У реализацији" || status == "U realizaciji") && ukupnoSati > limitSati && !potvrdaMenadzera)
            {
                return true;
            }

            return false;
        }

        // 3. Podrška za pripremu dokumenta za štampu
        public void PripremiZahtevZaStampu(int zahtevId)
        {
            // TODO: Logika za štampu
            throw new System.NotImplementedException();
        }
    }
}