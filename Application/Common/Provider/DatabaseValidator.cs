using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Provider
{
    public class DatabaseValidator
    {
        public static bool IsConnectionValid(string dataSource, string userId, string password)
        {
            // Create a connection string without Initial Catalog
            string connectionString = $"Data Source={dataSource};User Id={userId};Password={password};Encrypt=false;Connection Timeout=10;";
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                    return true; 
                }
            }
            catch (SqlException ex)
            {
                return false; 
            }
        }
    }
}
