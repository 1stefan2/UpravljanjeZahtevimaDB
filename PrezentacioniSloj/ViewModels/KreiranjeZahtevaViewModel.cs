using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PrezentacioniSloj.ViewModels
{
    public class KreiranjeZahtevaViewModel
    {
        // Master deo (osnovni podaci o zahtevu, projektima i statusima)
        public ZahtevZaIzmenomViewModel Zahtev { get; set; } = new ZahtevZaIzmenomViewModel();

        // Detail deo (lista stavki procene radova po fazama)
        public List<StavkaZahtevaViewModel> Stavke { get; set; } = new List<StavkaZahtevaViewModel>();

        // Automatski izračunat zbir svih sati iz stavki (koristi se za proveru poslovnog pravila sa limitom X)
        public decimal UkupnoSati
        {
            get
            {
                return Stavke != null ? Stavke.Sum(s => s.ProcenjenoSati) : 0;
            }
        }

        // Padajuće liste i izabrani ID-jevi
        public List<SelectListItem> ListaKlijenata { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaKorisnika { get; set; } = new List<SelectListItem>();
        public int IzabranKlijentId { get; set; }
        public int IzabranKorisnikId { get; set; }
    }
}