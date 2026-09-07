using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrezentacioniSloj.ViewModels
{
    public class ZahtevZaIzmenomViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Broj zahteva je obavezan.")]
        [StringLength(20, ErrorMessage = "Maksimalno 20 karaktera.")]
        public string BrojZahteva { get; set; }

        [Required(ErrorMessage = "Datum podnošenja je obavezan.")]
        [DataType(DataType.Date)]
        public DateTime DatumPodnosenja { get; set; }

        [Required(ErrorMessage = "Izbor klijenta je obavezan.")]
        public int KlijentId { get; set; }
        public string NazivKlijenta { get; set; } 

        [Required(ErrorMessage = "Izbor korisnika je obavezan.")]
        public int KorisnikId { get; set; }

        [Required(ErrorMessage = "Naziv projekta je obavezan.")]
        [StringLength(150, ErrorMessage = "Maksimalno 150 karaktera.")]
        public string NazivProjekta { get; set; }

        [Required(ErrorMessage = "Polje 'Zahtev podneo' je obavezno.")]
        [StringLength(100, ErrorMessage = "Maksimalno 100 karaktera.")]
        public string ZahtevPodneo { get; set; }

        [Required(ErrorMessage = "Status je obavezan.")]
        [StringLength(30, ErrorMessage = "Maksimalno 30 karaktera.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Detaljan opis je obavezan.")]
        public string DetaljanOpis { get; set; }

        
        public string PoslovniRazlog { get; set; }

        public string UticajNaRok { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NoviDatumIsporuke { get; set; }

        public bool PotvrdaMenadzera { get; set; }

        
        public List<StavkaZahtevaViewModel> Stavke { get; set; } = new List<StavkaZahtevaViewModel>();
    }
}