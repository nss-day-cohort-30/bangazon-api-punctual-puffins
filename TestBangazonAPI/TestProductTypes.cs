﻿using System;
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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.GetAsync("/api/producttype");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Create_One_Delete_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                ProductType newProduct = new ProductType
                {
                    Name = "Thundershirt"
                };

                var newProductAsJSON = JsonConvert.SerializeObject(newProduct);
                /*
                    ACT
                */
                var response = await client.PostAsync("/api/producttype/",
                    new StringContent(newProductAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var newProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Matches(newProductType.Name, "Thundershirt");

                /*DELETE*/

                var deleteResponse = await client.DeleteAsync($"/api/producttype/{newProductType.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Get_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var getResponse = await client.GetAsync("/api/producttype/2");


                string getResponseBody = await getResponse.Content.ReadAsStringAsync();
                var getProductType = JsonConvert.DeserializeObject<ProductType>(getResponseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
                Assert.Matches(getProductType.Name, "Video Game");
            }
        }
        [Fact]
        public async Task Test_Update_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                ProductType updateProduct = new ProductType
                {
                    Name = "Dog Socks"
                };

            var updateProductAsJSON = JsonConvert.SerializeObject(updateProduct);
            /*
                ACT
            */
            var updateResponse = await client.PutAsync("/api/producttype/1",
                new StringContent(updateProductAsJSON, Encoding.UTF8, "application/json"));


            string updateResponseBody = await updateResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var getDogSocks = await client.GetAsync("/api/producttype/1");
            getDogSocks.EnsureSuccessStatusCode();

            string getDogSocksBody = await getDogSocks.Content.ReadAsStringAsync();
            ProductType newDogSocks = JsonConvert.DeserializeObject<ProductType>(getDogSocksBody);

            Assert.Equal(HttpStatusCode.OK, getDogSocks.StatusCode);
            Assert.Equal("Dog Socks", newDogSocks.Name);
        }
    }
        [Fact]
        public async Task Test_getOneFalse_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.GetAsync("/api/producttype/1000");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_updateOneFalse_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                   ARRANGE
               */
                ProductType updateProduct = new ProductType
                {
                    Name = "Dog Socks"
                };

                var falseUpdateProductAsJSON = JsonConvert.SerializeObject(updateProduct);
                /*
                    ACT
                */
                var falseUpdateResponse = await client.PutAsync("/api/producttype/1000",
                    new StringContent(falseUpdateProductAsJSON, Encoding.UTF8, "application/json"));


                string falseUpdateResponseBody = await falseUpdateResponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NotFound, falseUpdateResponse.StatusCode);
            }
        }
        [Fact]
        public async Task Test_deleteOneFalse_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var newResponse = await client.DeleteAsync("/api/producttype/1000");


                string newResponseBody = await newResponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NotFound, newResponse.StatusCode);
            }
        }
    }
}
