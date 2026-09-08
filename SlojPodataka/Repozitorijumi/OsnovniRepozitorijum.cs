using System.Collections.Generic;

namespace SlojPodataka.Repozitorijumi
{
    public abstract class OsnovniRepozitorijum<T> where T : class
    {
        protected readonly string _konekcioniString;

        protected OsnovniRepozitorijum(string konekcioniString)
        {
            _konekcioniString = konekcioniString;
        }

        public abstract List<T> DajSve();
        public abstract T DajPoId(int id);
        public abstract void Dodaj(T entitet);
        public abstract void Izmeni(T entitet);
        public abstract void Obrisi(int id);
    }
}