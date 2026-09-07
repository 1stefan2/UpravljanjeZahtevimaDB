using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlojPodataka.Entities;

namespace SlojPodataka.Repozitorijumi
{
    public class StavkaZahtevaRepo : OsnovniRepozitorijum<StavkaZahteva>
    {
        public StavkaZahtevaRepo(string konekcioniString) : base(konekcioniString)
        {
        }

       
        public List<StavkaZahteva> DajPoIdZahteva(int zahtevId)
        {
            throw new NotImplementedException();
        }
    }
}