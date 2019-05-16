using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class TestOrder
    {
        [Fact]
        public async Task Test_Get_Single_Order()
        {

            using ( var order = new APIClientProvider().Client)
            {
                var response = await order.GetAsync("/api/Order/3");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(3, orderResponse.CustomerId);
                Assert.NotNull(orderResponse);
            }
        }

        [Fact]
        public async Task Test_Get_All_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        //get with _include=customers
        [Fact]
       public async Task Test_Get_All_Orde_with_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?_include=customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_All_Order_with_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?_include=products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        //Get all Completed orders
        [Fact]
        public async Task Test_Get_All_Order_Completed_true()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?completed=true");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        //get all not completed orders
        [Fact]
        public async Task Test_Get_All_Order_Completed_False()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Order?completed=false");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Non_Existing_Get_Update_And_Delete_Order()
        {
            using (var order = new APIClientProvider().Client)
            {

                var response = await order.GetAsync("/api/Order/99999");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);



                //Checking Update
                Order UpdateToNewOrder = new Order
                {
                    
                    CustomerId = 1
                };
                var newOrderJSON = JsonConvert.SerializeObject(UpdateToNewOrder);


                response = await order.PutAsync(
                    $"api/Order/99999",
                    new StringContent(newOrderJSON, Encoding.UTF8, "application/json")
                );
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);


                //Checking Delete Part
                var deleteResponse = await order.DeleteAsync($"/api/Order/99999");
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Order()
        {
            using (var order = new APIClientProvider().Client)
            {
                Order orders = new Order
                {
                    CustomerId = 2
                };
                var testorderJSON = JsonConvert.SerializeObject(orders);

                var response = await order.PostAsync(
                    "/api/Order",
                    new StringContent(testorderJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(2, newOrder.CustomerId);


                //Checking Delete Part
                var deleteResponse = await order.DeleteAsync($"/api/Order/{newOrder.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Update_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Checking Update

                Order UpdateToNewOrder = new Order
                {
                    PaymentTypeId =1
                   
                };
                var newDeptJSON = JsonConvert.SerializeObject(UpdateToNewOrder);


                var response = await client.PutAsync(
                            $"api/Order/3",
                            new StringContent(newDeptJSON, Encoding.UTF8, "application/json")
                        );
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                //  GET single department 

                var getNewOrder = await client.GetAsync($"/api/Order/3");
                getNewOrder.EnsureSuccessStatusCode();

                string getNewOrderBody = await getNewOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(getNewOrderBody);

                Assert.Equal(HttpStatusCode.OK, getNewOrder.StatusCode);
                Assert.Equal(1, newOrder.PaymentTypeId);
            }
        }

    }
}
