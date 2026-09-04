using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;


namespace SlojPodataka.Entities
{
    [Table("Korisnik")]
    public class Korisnik
    {
        public int Id { get; set; }
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Lozinka { get; set; } = string.Empty;
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string Uloga { get; set; } = string.Empty;
        public string Email { get; set; } = null;

    }
}