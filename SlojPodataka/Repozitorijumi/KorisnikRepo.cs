using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SlojPodataka.Entities;

namespace SlojPodataka.Repozitorijumi
{
    public class KorisnikRepo : OsnovniRepozitorijum<Korisnik>
    {
        public KorisnikRepo(string konekcioniString) : base(konekcioniString)
        {
        }

        
        public override List<Korisnik> DajSve()
        {
            var korisnici = new List<Korisnik>();

            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKorisnik_GetAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            korisnici.Add(MapToKorisnik(reader));
                        }
                    }
                }
            }

            return korisnici;
        }

        
        public override Korisnik DajPoId(int id)
        {
            Korisnik korisnik = null;

            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKorisnik_GetById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            korisnik = MapToKorisnik(reader);
                        }
                    }
                }
            }

            return korisnik;
        }

        
        public override void Dodaj(Korisnik entity)
        {
            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKorisnik_Add", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@KorisnickoIme", entity.KorisnickoIme);
                    command.Parameters.AddWithValue("@Lozinka", entity.Lozinka);
                    command.Parameters.AddWithValue("@Ime", entity.Ime);
                    command.Parameters.AddWithValue("@Prezime", entity.Prezime);
                    command.Parameters.AddWithValue("@Uloga", entity.Uloga);
                    command.Parameters.AddWithValue("@Email", (object)entity.Email ?? DBNull.Value); 

                    connection.Open();

                    
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        entity.Id = newId;
                    }
                }
            }
        }

        
        public override void Izmeni(Korisnik entitet)
        {
            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKorisnik_Update", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Id", entitet.Id);
                    command.Parameters.AddWithValue("@KorisnickoIme", entitet.KorisnickoIme);
                    command.Parameters.AddWithValue("@Lozinka", entitet.Lozinka);
                    command.Parameters.AddWithValue("@Ime", entitet.Ime);
                    command.Parameters.AddWithValue("@Prezime", entitet.Prezime);
                    command.Parameters.AddWithValue("@Uloga", entitet.Uloga);
                    command.Parameters.AddWithValue("@Email", (object)entitet.Email ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

       
        public override void Obrisi(int id)
        {
            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKorisnik_Delete", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        
        public Korisnik DajPoKorisnickomImenuILozinci(string korisnickoIme, string lozinka)
        {
            Korisnik korisnik = null;

            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKorisnik_Prijava", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@KorisnickoIme", korisnickoIme);
                    command.Parameters.AddWithValue("@Lozinka", lozinka);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            korisnik = MapToKorisnik(reader);
                        }
                    }
                }
            }

            return korisnik;
        }

        
        private Korisnik MapToKorisnik(SqlDataReader reader)
        {
            return new Korisnik
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                KorisnickoIme = reader.GetString(reader.GetOrdinal("KorisnickoIme")),
                Lozinka = reader.GetString(reader.GetOrdinal("Lozinka")),
                Ime = reader.GetString(reader.GetOrdinal("Ime")),
                Prezime = reader.GetString(reader.GetOrdinal("Prezime")),
                Uloga = reader.GetString(reader.GetOrdinal("Uloga")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
            };
        }
    }
}