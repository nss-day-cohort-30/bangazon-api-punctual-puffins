using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestProducts
    {

        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Product");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Product/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, product.ProductTypeId);
                Assert.Equal(3, product.CustomerId);
                Assert.Equal(23.3000M, product.Price);
                Assert.Equal("super big frying pan", product.Title);
                Assert.Equal("jstainless steel frying pan 12 in", product.Description);
                Assert.Equal(1, product.Quantity);
                Assert.NotNull(product);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Product_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Product/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product product = new Product
                {
                    ProductTypeId = 4,
                    CustomerId = 4,
                    Price = 7000.99M,
                    Title = "Super Hammock",
                    Description = "just wear it and stuff",
                    Quantity = 1
                };
                var productAsJSON = JsonConvert.SerializeObject(product);

                var response = await client.PostAsync(
                    "/api/Product",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(4, newProduct.ProductTypeId);
                Assert.Equal(4, newProduct.CustomerId);
                Assert.Equal("Super Hammock", newProduct.Title);
                Assert.Equal("just wear it and stuff", newProduct.Description);
                Assert.Equal(1, newProduct.Quantity);

                var deleteResponse = await client.DeleteAsync($"/api/Product/{newProduct.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Product_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/Product/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Student()
        {
            string newTitle = "I am a different title";

            using (var client = new APIClientProvider().Client)
            {
                Product modifiedProduct = new Product
                {
                    ProductTypeId = 4,
                    CustomerId = 4,
                    Price = 7000.99M,
                    Title = newTitle,
                    Description = "just wear it and stuff",
                    Quantity = 1
                };
                var modifiedProductAsJSON = JsonConvert.SerializeObject(modifiedProduct);

                var response = await client.PutAsync(
                    "api/Product/1",
                    new StringContent(modifiedProductAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getProduct = await client.GetAsync("api/Product/1");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getProduct.StatusCode);
                Assert.Equal(newTitle, newProduct.Title);
            }
        }
    }
}
