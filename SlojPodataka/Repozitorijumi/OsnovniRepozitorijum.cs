using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Repozitorijumi // Исправљено
{
    public class OsnovniRepozitorijum<T> where T : class // Исправљено
    {
        // Polje za konekcioni string (ili DbContext) dostupno svim izvedenim klasama
        protected readonly string _konekcioniString; // Исправљено

        public OsnovniRepozitorijum(string konekcioniString) // Исправљено
        {
            _konekcioniString = konekcioniString;
            
        }

        public virtual T DajPoId(int id) // Исправљено
        {
            throw new NotImplementedException();
        }

        public virtual List<T> DajSve()
        {
            throw new NotImplementedException();
        }

        public virtual void Dodaj(T entitet) // Исправљено
        {
            throw new NotImplementedException();
        }

        public virtual void Izmeni(T entitet) // Исправљено
        {
            throw new NotImplementedException();
        }

        public virtual void Obrisi(int id)
        {
            throw new NotImplementedException();
        }
    }
}