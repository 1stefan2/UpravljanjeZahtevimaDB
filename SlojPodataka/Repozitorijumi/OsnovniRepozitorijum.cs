using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlojPodataka.Repozitorijumi 
{
    public class OsnovniRepozitorijum<T> where T : class 
    {
        
        protected readonly string _konekcioniString; 

        public OsnovniRepozitorijum(string konekcioniString) 
        {
            _konekcioniString = konekcioniString;
            
        }

        public virtual T DajPoId(int id) 
        {
            throw new NotImplementedException();
        }

        public virtual List<T> DajSve()
        {
            throw new NotImplementedException();
        }

        public virtual void Dodaj(T entitet) 
        {
            throw new NotImplementedException();
        }

        public virtual void Izmeni(T entitet) 
        {
            throw new NotImplementedException();
        }

        public virtual void Obrisi(int id)
        {
            throw new NotImplementedException();
        }
    }
}