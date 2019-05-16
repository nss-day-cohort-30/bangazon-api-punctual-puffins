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
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrderController(IConfiguration config)
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
        public async Task<IActionResult> Get(string _include, bool completed )
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (_include == "customers")
                    {
                        cmd.CommandText = @"SELECT o.Id, 
                                           c.FirstName, 
                                           c.LastName, 
                                           c.Id custId,
                                           o.PaymentTypeId
                                        FROM [Order] o JOIN Customer c
                                        ON o.CustomerId = c.Id";

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();


                        List<Order> orders = new List<Order>();
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                Customer = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("custId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))

                                }

                            });
                        }
                        reader.Close();
                        return Ok(orders);
                    }
                    else
                   if (_include == "products")
                    {
                        cmd.CommandText = @"SELECT o.Id,
                                                o.CustomerId CustId,
                                                o.PaymentTypeId,
                                                op.Id opId, 
                                                op.ProductId ProdId, 
                                                p.Title, 
                                                p.Quantity, 
                                                p.[Description] Descrip, 
                                                pt.[Name] PType,
                                                p.ProductTypeId PtId,
                                                p.Price
                                            FROM [Order] o Left Join orderProduct op on o.Id = op.OrderId
                                            JOIN Product p ON p.Id = op.ProductId
                                            JOIN ProductType pt ON op.ProductId = pt.Id";

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Order> HashTable = new Dictionary<int, Order>();
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!HashTable.ContainsKey(orderId))
                            {
                                Order order = new Order
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustId")),
                                    PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                                };
                                HashTable[orderId] = order;
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("opId")))
                            {

                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProdId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Descrip")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("PtId")),
                                    ProductType = new ProductType
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("PtId")),
                                        Name = reader.GetString(reader.GetOrdinal("PType"))
                                    }
                                };
                                HashTable[orderId].Products.Add(product);
                            }
                        }

                        reader.Close();
                        List<Order> orders = HashTable.Values.ToList();

                        return Ok(orders);
                    }
                    else
                    if (completed == false)
                    {

                        cmd.CommandText = @"SELECT o.Id, 
                                            o.CustomerId,
                                            o.PaymentTypeId
                                    FROM [Order] o 
                                    WHERE o.PaymentTypeId IS NULL";

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();


                        List<Order> orders = new List<Order>();
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            });
                        }
                        reader.Close();
                        return Ok(orders);
                    }
                    else
                    {
                        if (completed == true)
                        {

                            cmd.CommandText = @"SELECT o.Id, 
                                            o.CustomerId,
                                            o.PaymentTypeId
                                    FROM [Order] o 
                                    WHERE o.PaymentTypeId IS NOT NULL";

                            SqlDataReader reader = await cmd.ExecuteReaderAsync();


                            List<Order> orders = new List<Order>();
                            while (reader.Read())
                            {
                                orders.Add(new Order
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                });
                            }
                            reader.Close();
                            return Ok(orders);
                        }

                        else
                        {
                            cmd.CommandText = @"SELECT o.Id, 
                                           o.CustomerId,
                                           o.PaymentTypeId
                                        FROM [Order] o";

                            SqlDataReader reader = await cmd.ExecuteReaderAsync();


                            List<Order> orders = new List<Order>();
                            while (reader.Read())
                            {
                                orders.Add(new Order
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                });
                            }
                            reader.Close();
                            return Ok(orders);
                        }
                    }

                }
            }
        }

        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            if (OrderExists(id))
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {


                        cmd.CommandText = @"SELECT o.Id,
                                                o.CustomerId CustId,
                                                o.PaymentTypeId,
                                                op.Id opId, 
                                                op.ProductId ProdId, 
                                                p.Title, 
                                                p.Quantity, 
                                                p.[Description] Descrip, 
                                                pt.[Name] PType,
                                                p.ProductTypeId PtId,
                                                p.Price
                                            FROM [Order] o Left Join orderProduct op on o.Id = op.OrderId
                                            LEFT JOIN Product p ON p.Id = op.ProductId
                                            LEFT JOIN ProductType pt ON op.ProductId = pt.Id
                                            WHERE o.Id = @custId";
                        cmd.Parameters.Add(new SqlParameter("@custId", id));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Dictionary<int, Order> HashTable = new Dictionary<int, Order>();
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32(reader.GetOrdinal("Id"));
                            if (!HashTable.ContainsKey(orderId))
                            {
                                Order order = new Order
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustId")),
                                    PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0 : reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                                };
                                HashTable[orderId] = order;
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("opId")))
                            {

                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProdId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Descrip")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("PtId")),
                                    ProductType = new ProductType
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("PtId")),
                                        Name = reader.GetString(reader.GetOrdinal("PType"))
                                    }
                                };
                                HashTable[orderId].Products.Add(product);
                            }
                        }

                        reader.Close();
                        return Ok(HashTable[id]);
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
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                
                    cmd.CommandText = @"
                        INSERT INTO [Order] (CustomerId) 
                        OUTPUT INSERTED.Id
                        VALUES (@custId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@custId", order.CustomerId));

                    order.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE [Order]
                            SET PaymentTypeId = @payId
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@payId", order.PaymentTypeId));
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
                if (!OrderExists(id))
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
                        if (OrderExists(id))
                        {
                                cmd.CommandText = @"
                                                 DELETE FROM OrderProduct
                                                 WHERE OrderId = @id";
                                cmd.Parameters.Add(new SqlParameter("@id", id));

                               int  rowsAffected = await cmd.ExecuteNonQueryAsync();

                            cmd.CommandText = @"
                                            DELETE FROM [Order] 
                                            WHERE Id = @orderId";
                            cmd.Parameters.Add(new SqlParameter("@orderId", id));

                            rowsAffected = await cmd.ExecuteNonQueryAsync();


                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        else
                            throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
                else
                {
                    throw;
                }
            }

        }

        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM [Order] WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}