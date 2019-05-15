<<<<<<< HEAD:BangazonAPI/Controllers/CustomersController.cs
﻿using System;
using System.Collections.Generic;
using System.Data;
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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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
                    cmd.CommandText = "Write your SQL statement here to get all customers";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Customer> customers = new List<Customer>();
                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            // You might have more columns
                        };

                        customers.Add(customer);
                    }

                    reader.Close();

                    return Ok(customers);
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
                    cmd.CommandText = "Write your SQL statement here to get a single customer";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Customer customer = null;
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            // You might have more columns
                        };
                    }

                    reader.Close();

                    return Ok(customer);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer ()
                        OUTPUT INSERTED.Id
                        VALUES ()
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

                    customer.Id = (int) await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @firstName
                            -- Set the remaining columns here
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

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
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE api/values/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //} 

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Data;
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
    public class ProductTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductTypeController(IConfiguration config)
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
                    cmd.CommandText = "SELECT Id, Name FROM ProductType";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<ProductType> productTypes = new List<ProductType>();
                    while (reader.Read())
                    {
                        ProductType productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };

                        productTypes.Add(productType);
                    }

                    reader.Close();

                    return Ok(productTypes);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetProductType")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ProductTypeExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM ProductType WHERE @id = ProductType.Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    ProductType productType = null;
                    if (reader.Read())
                    {
                        productType = new ProductType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                    }

                    reader.Close();

                    return Ok(productType);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductType productType)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO ProductType (Name)
                        OUTPUT INSERTED.Id
                        VALUES (@name)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@name", productType.Name));

                    int newId = (int)await cmd.ExecuteScalarAsync();

                    productType.Id = newId;

                    return CreatedAtRoute("GetProductType", new { id = newId }, productType);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductType productType)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE ProductType
                            SET Name = @name
                            WHERE productType.Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@name", productType.Name));

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
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ProductTypeExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM ProductType WHERE Id = @id";
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
    



        private bool ProductTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM ProductType WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
>>>>>>> master:BangazonAPI/Controllers/ProductTypeController.cs
