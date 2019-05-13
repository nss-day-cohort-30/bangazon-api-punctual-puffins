using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    public class ValidationController : ControllerBase
    {
        private readonly IConfiguration _config;

       
        public ValidationController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<bool> Duplicate<T>(string dbName, string columnName, T columnValue)
        {
            var result = false;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT @columnName FROM @dbName WHERE @columnName = @columnValue";
                    cmd.Parameters.Add(new SqlParameter("@columnName", columnName));
                    cmd.Parameters.Add(new SqlParameter("@columnValue", columnValue));
                    cmd.Parameters.Add(new SqlParameter("@dbName", dbName));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                        result= true;
                }
            }
            return result;
        }
    }
}