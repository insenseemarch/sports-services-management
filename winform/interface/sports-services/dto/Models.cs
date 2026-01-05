using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsServices.Dto
{

}
public static class DbHelper
{
    private static string _connectionString =
        @"Data Source=.\SQLEXPRESS;Initial Catalog=QL_DatSan;Integrated Security=True";

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        using (var conn = GetConnection())
        using (var cmd = new SqlCommand(sql, conn))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }

    public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
    {
        using (var conn = GetConnection())
        using (var cmd = new SqlCommand(sql, conn))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
