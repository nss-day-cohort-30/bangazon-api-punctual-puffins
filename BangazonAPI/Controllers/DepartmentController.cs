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
                                        FROM Department d LEFT JOIN Employee e
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
                        Console.WriteLine(reader.IsDBNull(reader.GetOrdinal("empId")));
                        //if (reader.GetString(reader.GetOrdinal("FirstName")) != null)
                        if (!reader.IsDBNull(reader.GetOrdinal("empId")))
                        {
                            
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("empId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Supervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"))
                            };
                            HashTable[DeptId].Employees.Add(employee);
                        }
                    }

                    reader.Close();
                    List<Department> departments = HashTable.Values.ToList();

                    return Ok(departments);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get(int id)
        {
            if (DepartmentExists(id))
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
                                        FROM Department d LEFT JOIN Employee e
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
                            if (!reader.IsDBNull(reader.GetOrdinal("empId")))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("empId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                };
                                HashTable[DeptId].Employees.Add(employee);
                            }
                        }

                        reader.Close();
                        department = HashTable[id];

                        return Ok(department);
                    }
                }
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            
        }
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Department ([Name], Budget) 
                        OUTPUT INSERTED.Id
                        VALUES (@name, @budget)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    department.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetDepartment", new { id = department.Id }, department);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Department
                            SET Name = @name,
                                Budget = @budget
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!DepartmentExists(id))
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    throw;
                }
            }
        }
        
        //DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                        DELETE FROM Department
                        WHERE Id = @id
                    ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!DepartmentExists(id))
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    throw;
                }
            }
        
        }

        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}