using SlojPodataka.Entities;
using SlojPodataka.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SlojPodataka.Repozitorijumi
{
    public class KlijentRepo : DBUtils<Klijent>
    {
        public KlijentRepo(string konekcioniString) : base(konekcioniString)
        {
            // Поље _dbUtils је уклоњено, а конструктор само прослеђује стринг родитељу
        }

        public override List<Klijent> DajSve()
        {
            var klijenti = new List<Klijent>();

            
            DataTable tabela = IzvrsiUpit("spKlijent_GetAll");

            foreach (DataRow red in tabela.Rows)
            {
                klijenti.Add(MapirajRedUKlijent(red));
            }

            return klijenti;
        }

        public override Klijent DajPoId(int id)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@Id", id)
            };

            DataTable tabela = IzvrsiUpit("spKlijent_GetById", parametri);

            if (tabela.Rows.Count > 0)
            {
                return MapirajRedUKlijent(tabela.Rows[0]);
            }

            return null;
        }

        public override void Dodaj(Klijent entitet)
        {
            SqlParameter[] parametri = {
        new SqlParameter("@NazivFirme", entitet.NazivFirme),
        new SqlParameter("@Kontakt", entitet.Kontakt),
        new SqlParameter("@Telefon", (object)entitet.Telefon ?? DBNull.Value),
        new SqlParameter("@Email", (object)entitet.Email ?? DBNull.Value)
    };

            
            object rezultat = IzvrsiSkalar("spKlijent_Add", parametri);

            
            if (rezultat != null && int.TryParse(rezultat.ToString(), out int noviId))
            {
                entitet.Id = noviId;
            }
        }

        public override void Izmeni(Klijent entitet)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@Id", entitet.Id),
                new SqlParameter("@Naziv", entitet.NazivFirme),
                new SqlParameter("@Kontakt", entitet.Kontakt),
                new SqlParameter("@Telefon", (object)entitet.Telefon ?? DBNull.Value),
                new SqlParameter("@Email", (object)entitet.Email ?? DBNull.Value)
            };

            
            IzvrsiKomandu("spKlijent_Update", parametri);
        }

        public override void Obrisi(int id)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@Id", id)
            };

            IzvrsiKomandu("spKlijent_Delete", parametri);
        }

        private Klijent MapirajRedUKlijent(DataRow red)
        {
            return new Klijent
            {
                Id = Convert.ToInt32(red["Id"]),
                NazivFirme = red["NazivFirme"].ToString(),
                Kontakt = red["Kontakt"].ToString(),
                Telefon = red["Telefon"] == DBNull.Value ? null : red["Telefon"].ToString(),
                Email = red["Email"] == DBNull.Value ? null : red["Email"].ToString()
            };
        }
    }
}