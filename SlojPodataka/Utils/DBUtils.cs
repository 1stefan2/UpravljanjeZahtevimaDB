using System;
using System.Data;
using System.Data.SqlClient;

namespace SlojPodataka.Helpers
{
    public class DBUtils
    {
        private readonly string _konekcioniString;

        public DBUtils(string konekcioniString)
        {
            _konekcioniString = konekcioniString;
        }

        // Pomoćna metoda za upite koji vraćaju podatke (SELECT -> DataTable)
        public DataTable ExecuteQuery(string procedureName, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        // Pomoćna metoda za akcione upite (INSERT, UPDATE, DELETE -> ExecuteNonQuery)
        public int ExecuteNonQuery(string procedureName, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_konekcioniString))
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}