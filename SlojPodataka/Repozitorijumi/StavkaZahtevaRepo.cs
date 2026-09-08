using SlojPodataka.Entities; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SlojPodataka.Helpers;

namespace SlojPodataka.Repozitorijumi
{
    public class StavkaZahtevaRepo : DBUtils<StavkaZahteva>
    {
        public StavkaZahtevaRepo(string konekcioniString) : base(konekcioniString)
        {
        }

        public override List<StavkaZahteva> DajSve()
        {
            var stavke = new List<StavkaZahteva>();
            DataTable tabela = IzvrsiUpit("spStavkaZahteva_GetAll");

            foreach (DataRow red in tabela.Rows)
            {
                stavke.Add(MapirajRedUStavku(red));
            }

            return stavke;
        }

        public override StavkaZahteva DajPoId(int id)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@Id", id)
            };

            DataTable tabela = IzvrsiUpit("spStavkaZahteva_GetById", parametri);

            if (tabela.Rows.Count > 0)
            {
                return MapirajRedUStavku(tabela.Rows[0]);
            }

            return null;
        }

        public override void Dodaj(StavkaZahteva entitet)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@ZahtevId", entitet.ZahtevId),
                new SqlParameter("@FazaVrstaPosla", entitet.FazaVrstaPosla),
                new SqlParameter("@ProcenjeniSati", entitet.ProcenjeniSati)
            };

            // Ако процедура не враћа нови ИД (ExecuteNonQuery)
            IzvrsiKomandu("spStavkaZahteva_Add", parametri);

            /* Ако процедура враћа нови ИД (ExecuteScalar), користи овај блок уместо линије изнад:
            using (SqlConnection konekcija = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand komanda = new SqlCommand("spStavkaZahteva_Add", konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;
                    komanda.Parameters.AddRange(parametri);
                    konekcija.Open();
                    object rezultat = komanda.ExecuteScalar();
                    if (rezultat != null && int.TryParse(rezultat.ToString(), out int noviId))
                    {
                        entitet.Id = noviId;
                    }
                }
            }
            */
        }

        public override void Izmeni(StavkaZahteva entitet)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@Id", entitet.Id),
                new SqlParameter("@ZahtevId", entitet.ZahtevId),
                new SqlParameter("@FazaVrstaPosla", entitet.FazaVrstaPosla),
                new SqlParameter("@ProcenjeniSati", entitet.ProcenjeniSati)
            };

            IzvrsiKomandu("spStavkaZahteva_Update", parametri);
        }

        public override void Obrisi(int id)
        {
            SqlParameter[] parametri = {
                new SqlParameter("@Id", id)
            };

            IzvrsiKomandu("spStavkaZahteva_Delete", parametri);
        }

        
        private StavkaZahteva MapirajRedUStavku(DataRow red)
        {
            return new StavkaZahteva
            {
                Id = Convert.ToInt32(red["Id"]),
                ZahtevId = Convert.ToInt32(red["ZahtevId"]),
                FazaVrstaPosla = red["FazaVrstaPosla"].ToString(),
                ProcenjeniSati = Convert.ToDecimal(red["ProcenjeniSati"])
            };
        }
        public List<StavkaZahteva> DajPoIdZahteva(int zahtevId)
        {
            var stavke = new List<StavkaZahteva>();
            SqlParameter[] parametri = {
        new SqlParameter("@ZahtevId", zahtevId)
    };

            DataTable tabela = IzvrsiUpit("spStavkaZahteva_GetByZahtevId", parametri);

            foreach (DataRow red in tabela.Rows)
            {
                stavke.Add(MapirajRedUStavku(red));
            }

            return stavke;
        }
    }
}