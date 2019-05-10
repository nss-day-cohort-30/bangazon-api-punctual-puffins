using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, 
                                           d.Name, 
                                            d.Budget, 
                                            e.Id empId,
                                            e.FirstName,
                                            e.LastName,
                                            e.IsSupervisor
                                        FROM Department d JOIN Employee e
                                        ON d.Id = e.DepartmentId";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    
                    Dictionary<int, Department> HashTable = new Dictionary<int, Department>();
                    while (reader.Read())
                    {
                        int DeptId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!HashTable.ContainsKey(DeptId))
                        {
                            Department dept = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                // You might have more columns
                            };
                            HashTable[DeptId] = dept;
                        };
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("empId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            //Supervisor = reader.GetInt32(reader.GetOrdinal("Supervisor"))
                        };
                        HashTable[DeptId].Employees.Add(employee);
                    }

                    reader.Close();
                    List<Department> departments = HashTable.Values.ToList();

                    return Ok(departments);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT d.Id, 
                                           d.Name, 
                                            d.Budget, 
                                            e.Id empId,
                                            e.FirstName,
                                            e.LastName,
                                            e.IsSupervisor
                                        FROM Department d JOIN Employee e
                                        ON d.Id = e.DepartmentId
                                        WHERE d.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Department department = null;
                    Dictionary<int, Department> HashTable = new Dictionary<int, Department>();
                    while (reader.Read())
                    {
                        int DeptId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!HashTable.ContainsKey(DeptId))
                        {
                            Department dept = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                // You might have more columns
                            };
                            HashTable[DeptId] = dept;
                        };
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("empId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            //Supervisor = reader.GetInt32(reader.GetOrdinal("Supervisor"))
                        };
                        HashTable[DeptId].Employees.Add(employee);
                    }

                    reader.Close();
                    department = HashTable[id];

                    return Ok(department);
                }
            }
        }

    }
}