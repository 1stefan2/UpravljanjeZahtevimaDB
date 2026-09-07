using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Entities
{
    public class ZahtevZaIzmenom
    {
        public int Id { get; set; }
        public string BrojZahteva { get; set; } = string.Empty;
        public DateTime DatumPodnosenja { get; set; }
        public int KlijentId { get; set; }
        public int KorisnikId { get; set; }
        public string NazivProjekta { get; set; } = string.Empty;
        public string ZahtevPodneo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DetaljanOpis { get; set; } = string.Empty;
        public string PoslovniRazlog { get; set; } = null;
        public string UticajNaRok { get; set; } = null;
        public DateTime? NoviDatumIsporuke { get; set; }
        public bool PotvrdaMenadzera { get; set; }

        public Klijent Klijent { get; set; } = null;
        public Korisnik Korisnik { get; set; } = null;
        public List<StavkaZahteva> Stavke { get; set; } = new List<StavkaZahteva>();
    }
}