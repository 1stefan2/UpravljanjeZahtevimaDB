using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SlojPodataka.Entities;
using SlojPodataka.PomocneKlase; 

namespace SlojPodataka.Repozitorijumi
{
    public class KorisnikRepo : OsnovniRepozitorijum<Korisnik>
    {
        private readonly SkladistenaProcedurama _skladistena;

        public KorisnikRepo(string konekcioniString) : base(konekcioniString)
        {
           
            _skladistena = new SkladistenaProcedurama(konekcioniString);
        }

        public override List<Korisnik> DajSve()
        {
            var korisnici = new List<Korisnik>();
            DataTable dt = _skladistena.IzvrsiUpit("spKorisnik_GetAll");

            foreach (DataRow row in dt.Rows)
            {
                korisnici.Add(MapToKorisnik(row));
            }

            return korisnici;
        }

        public override Korisnik DajPoId(int id)
        {
            SqlParameter[] parametri = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };

            DataTable dt = _skladistena.IzvrsiUpit("spKorisnik_GetById", parametri);

            if (dt.Rows.Count > 0)
            {
                return MapToKorisnik(dt.Rows[0]);
            }

            return null;
        }

        public override void Dodaj(Korisnik entity)
        {
            SqlParameter[] parametri = new SqlParameter[]
            {
                new SqlParameter("@KorisnickoIme", entity.KorisnickoIme),
                new SqlParameter("@Lozinka", entity.Lozinka),
                new SqlParameter("@Ime", entity.Ime),
                new SqlParameter("@Prezime", entity.Prezime),
                new SqlParameter("@Uloga", entity.Uloga),
                new SqlParameter("@Email", (object)entity.Email ?? DBNull.Value)
            };

            
            DataTable dt = _skladistena.IzvrsiUpit("spKorisnik_Add", parametri);

            if (dt.Rows.Count > 0 && int.TryParse(dt.Rows[0][0].ToString(), out int newId))
            {
                entity.Id = newId;
            }
        }

        public override void Izmeni(Korisnik entitet)
        {
            SqlParameter[] parametri = new SqlParameter[]
            {
                new SqlParameter("@Id", entitet.Id),
                new SqlParameter("@KorisnickoIme", entitet.KorisnickoIme),
                new SqlParameter("@Lozinka", entitet.Lozinka),
                new SqlParameter("@Ime", entitet.Ime),
                new SqlParameter("@Prezime", entitet.Prezime),
                new SqlParameter("@Uloga", entitet.Uloga),
                new SqlParameter("@Email", (object)entitet.Email ?? DBNull.Value)
            };

            _skladistena.IzvrsiKomandu("spKorisnik_Update", parametri);
        }

        public override void Obrisi(int id)
        {
            SqlParameter[] parametri = new SqlParameter[]
            {
                new SqlParameter("@Id", id)
            };

            _skladistena.IzvrsiKomandu("spKorisnik_Delete", parametri);
        }

        public Korisnik DajPoKorisnickomImenuILozinci(string korisnickoIme, string lozinka)
        {
            SqlParameter[] parametri = new SqlParameter[]
            {
                new SqlParameter("@KorisnickoIme", korisnickoIme),
                new SqlParameter("@Lozinka", lozinka)
            };

            DataTable dt = _skladistena.IzvrsiUpit("spKorisnik_Prijava", parametri);

            if (dt.Rows.Count > 0)
            {
                return MapToKorisnik(dt.Rows[0]);
            }

            return null;
        }

        // Metoda sada mapira iz DataRow objekta umesto iz SqlDataReader-a
        private Korisnik MapToKorisnik(DataRow row)
        {
            return new Korisnik
            {
                Id = Convert.ToInt32(row["Id"]),
                KorisnickoIme = row["KorisnickoIme"].ToString(),
                Lozinka = row["Lozinka"].ToString(),
                Ime = row["Ime"].ToString(),
                Prezime = row["Prezime"].ToString(),
                Uloga = row["Uloga"].ToString(),
                Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString()
            };
        }
    }
}