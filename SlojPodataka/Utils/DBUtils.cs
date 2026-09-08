using System.Data;
using System.Data.SqlClient;

namespace SlojPodataka.Helpers
{
    public class DBUtils
    {
        
        protected readonly string _konekcioniString;

        public DBUtils(string konekcioniString)
        {
            _konekcioniString = konekcioniString;
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
    }
}