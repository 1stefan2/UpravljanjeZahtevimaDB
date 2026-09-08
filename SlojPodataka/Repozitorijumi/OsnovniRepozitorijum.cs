using System.Collections.Generic;
using SlojPodataka.Helpers; 

namespace SlojPodataka.Repozitorijumi
{
    public abstract class OsnovniRepozitorijum<T> : DBUtils where T : class
    {
        

        public OsnovniRepozitorijum(string konekcioniString) : base(konekcioniString)
        {
            
        }

        public abstract T DajPoId(int id);
        public abstract List<T> DajSve();
        public abstract void Dodaj(T entitet);
        public abstract void Izmeni(T entitet);
        public abstract void Obrisi(int id);
    }
}