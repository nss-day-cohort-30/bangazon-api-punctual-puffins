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
    public class TestComputers
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync("/api/computers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Modify_Computer()
        {
            // New last name to change to and test
            string newMake = "Matebook X";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                Computer modifiedHuawei = new Computer
                {
                    PurchaseDate = DateTime.Parse ("2015-05-12 12:00:00"),
                    DecomissionDate = DateTime.Parse ("2019-05-13 17:00:00"),
                    Make = newMake,
                    Manufacturer = "Huawei"
                };
                var modifiedHuaweiAsJSON = JsonConvert.SerializeObject(modifiedHuawei);

                var response = await client.PutAsync(
                    "/api/computers/2",
                    new StringContent(modifiedHuaweiAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getHuawei = await client.GetAsync("/api/computers/2");
                getHuawei.EnsureSuccessStatusCode();

                string getHuaweiBody = await getHuawei.Content.ReadAsStringAsync();
                Computer newHuawei = JsonConvert.DeserializeObject<Computer>(getHuaweiBody);

                Assert.Equal(HttpStatusCode.OK, getHuawei.StatusCode);
                Assert.Equal(newMake, newHuawei.Make);
            }
        }

        [Fact]
        public async Task Test_Post_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
               
                Computer newTestComputer = new Computer
                {
                    PurchaseDate = DateTime.Parse("2019-01-01 12:00:00"),
                    DecomissionDate = DateTime.Parse("2019-12-15 17:00:00"),
                    Make = "MacBook Air",
                    Manufacturer = "Apple"
                };

                var newTestComputerAsJSON = JsonConvert.SerializeObject(newTestComputer);

                var response = await client.PostAsync(
                    "/api/computers/",
                    new StringContent(newTestComputerAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

            }

        }

        [Fact]
        public async Task Test_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync("/api/computers/14");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            }

        }
    }
}