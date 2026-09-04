using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SlojPodataka.Entities;
using SlojPodataka.Helpers;

namespace SlojPodataka.Repozitorijumi
{
    public class KlijentRepo : OsnovniRepozitorijum<Klijent>
    {
        private readonly DBUtils _dbUtils;

        public KlijentRepo(string konekcioniString) : base(konekcioniString)
        {
            // Koristimo nasleđeni _konekcioniString iz OsnovniRepozitorijum klase
            _dbUtils = new DBUtils(_konekcioniString);
        }

        public override List<Klijent> DajSve()
        {
            var klijenti = new List<Klijent>();
            DataTable table = _dbUtils.ExecuteQuery("spKlijent_GetAll");

            foreach (DataRow row in table.Rows)
            {
                klijenti.Add(MapirajRedUKlijent(row));
            }

            return klijenti;
        }

        public override Klijent DajPoId(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@Id", id)
            };

            DataTable table = _dbUtils.ExecuteQuery("spKlijent_GetById", parameters);

            if (table.Rows.Count > 0)
            {
                return MapirajRedUKlijent(table.Rows[0]);
            }

            return null;
        }

        public override void Dodaj(Klijent entitet)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@NazivFirme", entitet.NazivFirme),
                new SqlParameter("@Kontakt", entitet.Kontakt),
                new SqlParameter("@Telefon", (object)entitet.Telefon ?? DBNull.Value),
                new SqlParameter("@Email", (object)entitet.Email ?? DBNull.Value)
            };

            // Pošto Add vraća novi ID preko SCOPE_IDENTITY(), otvaramo konekciju i pozivamo ExecuteScalar
            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand("spKlijent_Add", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddRange(parameters);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int newId))
                    {
                        entitet.Id = newId;
                    }
                }
            }
        }

        public override void Izmeni(Klijent entity)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@Id", entity.Id),
                new SqlParameter("@Naziv", entity.NazivFirme),
                new SqlParameter("@Kontakt", entity.Kontakt),
                new SqlParameter("@Telefon", (object)entity.Telefon ?? DBNull.Value),
                new SqlParameter("@Email", (object)entity.Email ?? DBNull.Value)
            };

            _dbUtils.ExecuteNonQuery("spKlijent_Update", parameters);
        }

        public override void Obrisi(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@Id", id)
            };

            _dbUtils.ExecuteNonQuery("spKlijent_Delete", parameters);
        }

        private Klijent MapirajRedUKlijent(DataRow row)
        {
            return new Klijent
            {
                Id = Convert.ToInt32(row["Id"]),
                NazivFirme = row["NazivFirme"].ToString(),
                Kontakt = row["Kontakt"].ToString(),
                Telefon = row["Telefon"] == DBNull.Value ? null : row["Telefon"].ToString(),
                Email = row["Email"] == DBNull.Value ? null : row["Email"].ToString()
            };
        }
    }
}