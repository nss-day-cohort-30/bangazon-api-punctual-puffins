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
        public async Task<IActionResult> Get(string _filter, string _include, int? _gt)
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
                                PaymentTypeId = reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")) == true ? 0: reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
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
                                                o.CustomerId,
                                                o.PaymentTypeId,
                                                op.Id opId, 
                                                op.ProductId ProdId, 
                                                p.Title, 
                                                p.Quantity, 
                                                p.[Description], 
                                                p.Price
                                            FROM [Order] o Left Join orderProduct op on o.Id = op.OrderId
                                            JOIN Product p on p.Id = op.ProductId";

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
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                    PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                                };
                                HashTable[orderId] = order;
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("opId")))
                            {

                                Product product = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("opId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Price = reader.GetInt32(reader.GetOrdinal("Price"))
                                };
                                HashTable[orderId].Products.Add(product);
                            }
                        }

                        reader.Close();
                        List<Order> orders = HashTable.Values.ToList();

                        return Ok(orders);
                    }
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                }
            }
        }
    }
}