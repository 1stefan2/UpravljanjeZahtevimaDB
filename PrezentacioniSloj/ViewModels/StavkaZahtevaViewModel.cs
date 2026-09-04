using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrezentacioniSloj.ViewModels
{
    public class StavkaZahtevaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Faza je obavezna.")]
        [StringLength(100, ErrorMessage = "Maksimalno 100 karaktera.")]
        public string FazaIliVrstaPosla { get; set; }

        [Required(ErrorMessage = "Procenjeni sati su obavezni.")]
        [Range(0.1, 999, ErrorMessage = "Sati moraju biti između 0.1 i 999.")]
        public decimal ProcenjenoSati { get; set; }
    }
}