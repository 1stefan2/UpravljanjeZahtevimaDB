using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Entities
{
    public class StavkaZahteva
    {
        public int Id { get; set; }
        public int ZahtevId { get; set; }
        public int OznakaStavke { get; set; }
        public string FazaVrstaPosla { get; set; } = string.Empty;
        public decimal ProcenjeniSati { get; set; }
        public string Napomena { get; set; } = null;

        // Navigacioni property ka master entitetu
        public ZahtevZaIzmenom ZahtevZaIzmenom { get; set; } = null;
    }
}


