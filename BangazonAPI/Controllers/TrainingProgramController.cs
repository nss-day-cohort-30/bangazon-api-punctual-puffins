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
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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
        public async Task<IActionResult> Get(string _completed)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    
                        cmd.CommandText = @"Select tp.Id, 
                                                tp.Name, 
                                                tp.StartDate, 
                                                tp.EndDate, 
                                                tp.MaxAttendees, 
                                                e.Id empId, 
                                                e.FirstName, 
                                                e.LastName, 
                                                e.DepartmentId, 
                                                e.IsSuperVisor
                                                FROM TrainingProgram tp LEFT JOIN EmployeeTraining et on tp.Id = et.TrainingProgramId
                                                LEFT JOIN Employee e On e.Id = et.EmployeeId
                                                WHERE 1=1";
                    
                    if (_completed == "false")
                    {
                        cmd.CommandText += " AND Convert(varchar(10), StartDate,120) >= CONVERT(varchar(10),GETDATE(),120)";
                    }
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, TrainingProgram> HashTable = new Dictionary<int, TrainingProgram>();
                    while (reader.Read())
                    {
                        int tProgramId = reader.GetInt32(reader.GetOrdinal("Id"));
                        if (!HashTable.ContainsKey(tProgramId))
                        {
                            TrainingProgram tProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("Maxattendees")),
                            };
                            HashTable[tProgramId] = tProgram;

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
                            HashTable[tProgramId].Employees.Add(employee);
                        }
                    }

                    reader.Close();
                    List<TrainingProgram> departments = HashTable.Values.ToList();

                    return Ok(departments);
                }
            }
        }
    

        // GET api/values/5
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)
        {
            if (TrainingProgramExists(id))
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"Select tp.Id, 
                                                tp.Name, 
                                                tp.StartDate, 
                                                tp.EndDate, 
                                                tp.MaxAttendees, 
                                                e.Id empId, 
                                                e.FirstName, 
                                                e.LastName, 
                                                e.DepartmentId, 
                                                e.IsSuperVisor
                                                FROM TrainingProgram tp LEFT JOIN EmployeeTraining et on tp.Id = et.TrainingProgramId
                                                LEFT JOIN Employee e On e.Id = et.EmployeeId
                                                WHERE tp.Id=@id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        TrainingProgram department = null;
                        Dictionary<int, TrainingProgram> HashTable = new Dictionary<int, TrainingProgram>();
                        while (reader.Read())
                        {
                            int tProgramId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!HashTable.ContainsKey(tProgramId))
                            {
                                TrainingProgram tProgram = new TrainingProgram
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    MaxAttendees = reader.GetInt32(reader.GetOrdinal("Maxattendees")),
                                };
                                HashTable[tProgramId] = tProgram;

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
                                HashTable[tProgramId].Employees.Add(employee);
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
        public async Task<IActionResult> Post([FromBody] TrainingProgram tProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) 
                        OUTPUT INSERTED.Id
                        VALUES (@name, @startDate, @endDate, @maxAttendees)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@name", tProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@startDate", tProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", tProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@maxAttendees", tProgram.MaxAttendees));

                    tProgram.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetTrainingProgram", new { id = tProgram.Id }, tProgram);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TrainingProgram tProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE TrainingProgram
                            SET Name = @name,
                                StartDate = @startDate,
                                EndDate = @endDate,
                                MaxAttendees = @maxAttendees
                                WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@name", tProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", tProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", tProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", tProgram.MaxAttendees));
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
                if (!TrainingProgramExists(id))
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
                        DELETE FROM TrainingProgram
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
                if (!TrainingProgramExists(id))
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    throw;
                }
            }

        }

        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}