using System.Data;
using System.Data.SqlClient;
using SlojPodataka.Repozitorijumi;

namespace SlojPodataka.Helpers
{
    public abstract class DBUtils<T> : OsnovniRepozitorijum<T> where T : class
    {
        
        protected DBUtils(string konekcioniString) : base(konekcioniString)
        {
        }

        public DataTable IzvrsiUpit(string nazivProcedure, SqlParameter[] parametri = null)
        {
            DataTable tabela = new DataTable();

            using (SqlConnection konekcija = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand komanda = new SqlCommand(nazivProcedure, konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;

                    if (parametri != null)
                    {
                        komanda.Parameters.AddRange(parametri);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(komanda))
                    {
                        adapter.Fill(tabela);
                    }
                }
            }

            return tabela;
        }

        public int IzvrsiKomandu(string nazivProcedure, SqlParameter[] parametri = null)
        {
            using (SqlConnection konekcija = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand komanda = new SqlCommand(nazivProcedure, konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;

                    if (parametri != null)
                    {
                        komanda.Parameters.AddRange(parametri);
                    }

                    konekcija.Open();
                    return komanda.ExecuteNonQuery();
                }
            }
        }
        public object IzvrsiSkalar(string nazivProcedure, SqlParameter[] parametri = null)
        {
            using (SqlConnection konekcija = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand komanda = new SqlCommand(nazivProcedure, konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;

                    if (parametri != null)
                    {
                        komanda.Parameters.AddRange(parametri);
                    }

                    konekcija.Open();
                    return komanda.ExecuteScalar();
                }
            }
        }
    }
}