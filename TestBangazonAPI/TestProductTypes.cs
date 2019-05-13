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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


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
        public async Task Test_Get_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                

                /*
                    ACT
                */
                var response = await client.GetAsync($"/api/producttype/5");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Matches(productType.Name, "Banana Hammock");
            }
        }
        [Fact]
        public async Task Test_Create_Update_And_Delete_One_ProductType()
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
                var response = await client.PostAsync($"/api/producttype/", 
                    new StringContent(newProductAsJSON, Encoding.UTF8, "application/json"));


                string responseBody = await response.Content.ReadAsStringAsync();
                var newProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Matches(newProductType.Name, "Thundershirt");

                /* UPDATE CREATION */
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
                var updateResponse = await client.PutAsync($"/api/producttype/{newProductType.Id}",
                    new StringContent(updateProductAsJSON, Encoding.UTF8, "application/json"));


                string updateResponseBody = await updateResponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

                var getDogSocks = await client.GetAsync($"/api/producttype/{newProductType.Id}");
                getDogSocks.EnsureSuccessStatusCode();

                string getDogSocksBody = await getDogSocks.Content.ReadAsStringAsync();
                ProductType newDogSocks = JsonConvert.DeserializeObject<ProductType>(getDogSocksBody);

                Assert.Equal(HttpStatusCode.OK, getDogSocks.StatusCode);
                Assert.Equal("Dog Socks", newDogSocks.Name);


                /* DELETE CREATION*/
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var newResponse = await client.DeleteAsync($"/api/producttype/{newProductType.Id}");


                string newResponseBody = await newResponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, newResponse.StatusCode);
            }
        }
        //[Fact]
        //public async Task Test_Edit_One_ProductType()
        //{
        //    using (var client = new APIClientProvider().Client)
        //    {
        //        /*
        //            ARRANGE
        //        */
        //        ProductType newProduct = new ProductType
        //        {
        //            Name = "Thundershirt"
        //        };

        //        var newProductAsJSON = JsonConvert.SerializeObject(newProduct);
        //        /*
        //            ACT
        //        */
        //        var response = await client.PutAsync($"/api/producttype/2", 
        //            new StringContent(newProductAsJSON, Encoding.UTF8, "application/json"));


        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //       var getThundershirt = await client.GetAsync("/api/producttype/2");
        //        getThundershirt.EnsureSuccessStatusCode();

        //        string getThundershirtBody = await getThundershirt.Content.ReadAsStringAsync();
        //        ProductType newThundershirt = JsonConvert.DeserializeObject<ProductType>(getThundershirtBody);

        //        Assert.Equal(HttpStatusCode.OK, getThundershirt.StatusCode);
        //        Assert.Equal("Thundershirt", newThundershirt.Name);
    //}
//}
    }
}
