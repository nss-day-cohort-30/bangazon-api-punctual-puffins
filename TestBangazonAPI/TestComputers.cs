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
        public async Task Test_Post_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {

               Computer newTestComputer = new Computer
                {
                    PurchaseDate = DateTime.Parse("2019-01-01 12:00:00"),
                    DecomissionDate = DateTime.Parse("2019-12-15 17:00:00"),
                    Make = "Blade Stealth 13",
                    Manufacturer = "Razer"
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
        public async Task Test_Modify_Computer()
        {
        
            using (var client = new APIClientProvider().Client)
            {
                
                string newMake = "NoteBook 9 Has Been Edited";
                string newModel = "Samsung Has Been Edited";

                Computer modifiedTestComputer = new Computer
                {
                    PurchaseDate = DateTime.Parse("2019-01-01 12:00:00"),
                    DecomissionDate = DateTime.Parse("2019-04-24 15:00:00"),
                    Make = newMake,
                    Manufacturer = newModel
                };
                var modifiedTestComputerAsJSON = JsonConvert.SerializeObject(modifiedTestComputer);

                var response = await client.PutAsync(
                   "/api/computers/1",
                   new StringContent(modifiedTestComputerAsJSON, Encoding.UTF8, "application/json")
               );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                
                var getComputer = await client.GetAsync($"/api/computers/1");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(newMake, newComputer.Make);
            }
        }

        [Fact]
        public async Task Test_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.DeleteAsync($"/api/computers/5");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }

        }
    }
}