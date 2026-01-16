using System.Data;
using System.Data.SqlClient;

namespace webapp_mvc.Models
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandTimeout = 120; // Increase timeout to 2 minutes
                
                // Auto detect SP: If query has no spaces and no newlines, assume it's an SP name
                if (!query.Trim().Contains(" ") && !query.Contains("\n")) 
                    cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var dt = new DataTable();
                var da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                return dt;
            }
        }

        public void ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                // SP or Text?
                if (!query.Contains(" ")) cmd.CommandType = CommandType.StoredProcedure;
                
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string query, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return default(T);
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }
}
