using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SlojPodataka.Entities
{
    
    public class Klijent
    {
        public int Id { get; set; }
        public string NazivFirme { get; set; } = string.Empty;
        public string Kontakt { get; set; } = null;
        public string Email { get; set; } = null;
        public string Telefon { get; set; } = null;


    }
}