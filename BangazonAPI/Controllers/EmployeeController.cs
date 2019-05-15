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
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeeController(IConfiguration config)
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
        public async Task<IActionResult> Get(string _filter, string _include, int? _gt)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "employees")
                    {
                        cmd.CommandText = @"SELECT e.Id, 
                                           e.FirstName, 
                                            e.LastName,
                                            e.IsSupervisor,
                                            d.Id employeeId,
                                            d.Name,
                                            d.Budget
                                        FROM Employee e LEFT JOIN Department d
                                        ON d.Id = e.EmployeeId
                                        WHERE 2 =2";
                        if (_filter == "budget")
                        {
                            cmd.CommandText += " AND d.Budget >= @gt";
                            cmd.Parameters.Add(new SqlParameter("@gt", _gt));
                        }
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();


                        Dictionary<int, Employee> HashTable = new Dictionary<int, Employee>();
                        while (reader.Read())
                        {
                            int DeptId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!HashTable.ContainsKey(DeptId))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                    // You might have more columns
                                };
                                HashTable[DeptId] = employee;
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
                        List<Employee> departments = HashTable.Values.ToList();

                        return Ok(departments);
                    }

                    else
                    {
                        cmd.CommandText = @"SELECT Id, 
                                           Name, 
                                            Budget
                                        FROM Employee
                                        WHERE 2 = 2";
                        if (_filter == "budget")
                        {
                            cmd.CommandText += " AND Budget >= @gt";
                            cmd.Parameters.Add(new SqlParameter("@gt", _gt));
                        }
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        List<Employee> departments = new List<Employee>();
                        while (reader.Read())
                        {
                            departments.Add(new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                            });
                        }
                        reader.Close();
                        return Ok(departments);
                    }
                    //return new StatusCodeResult(StatusCodes.Status204NoContent);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get(int id)
        {
            if (EmployeeExists(id))
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
                                        FROM Employee d LEFT JOIN Employee e
                                        ON d.Id = e.EmployeeId
                                        WHERE d.Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Employee department = null;
                        Dictionary<int, Employee> HashTable = new Dictionary<int, Employee>();
                        while (reader.Read())
                        {
                            int DeptId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!HashTable.ContainsKey(DeptId))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                    // You might have more columns
                                };
                                HashTable[DeptId] = employee;
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
        public async Task<IActionResult> Post([FromBody] Employee department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Employee ([Name], Budget) 
                        OUTPUT INSERTED.Id
                        VALUES (@name, @budget)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    department.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetEmployee", new { id = department.Id }, department);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Employee
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
                if (!EmployeeExists(id))
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
                        DELETE FROM Employee
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
                if (!EmployeeExists(id))
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    throw;
                }
            }

        }

        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}