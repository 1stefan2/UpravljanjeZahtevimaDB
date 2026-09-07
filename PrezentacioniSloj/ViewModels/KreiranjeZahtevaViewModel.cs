using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PrezentacioniSloj.ViewModels
{
    public class KreiranjeZahtevaViewModel
    {
        
        public ZahtevZaIzmenomViewModel Zahtev { get; set; } = new ZahtevZaIzmenomViewModel();

        
        public List<StavkaZahtevaViewModel> Stavke { get; set; } = new List<StavkaZahtevaViewModel>();

        public decimal UkupnoSati
        {
            get
            {
                return Stavke != null ? Stavke.Sum(s => s.ProcenjenoSati) : 0;
            }
        }

        public List<SelectListItem> ListaKlijenata { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaKorisnika { get; set; } = new List<SelectListItem>();
        public int IzabranKlijentId { get; set; }
        public int IzabranKorisnikId { get; set; }
    }
}