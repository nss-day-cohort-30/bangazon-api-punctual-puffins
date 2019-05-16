using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace TestBangazonAPI
{
    public class TestTrainingProgram
    {

        [Fact]
        public async Task Test_Get_Single_TrainingProgram()
        {

            using (var trainingProgram = new APIClientProvider().Client)
            {
                var response = await trainingProgram.GetAsync("/api/TrainingProgram/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgramResponse = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Python Party", trainingProgramResponse.Name);
                Assert.NotNull(trainingProgramResponse);
            }
        }
        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/TrainingProgram");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }
        // get all trainingProgram with employees
        [Fact]
        public async Task Test_Get_All_TrainingPrograms_Not_Completed()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/TrainingProgram?_completed=false");


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms[0].StartDate >= DateTime.Now);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram tProgram = new TrainingProgram
                {
                    Name = "Jump Java and Whales!",
                    StartDate = DateTime.Parse("2020-01-01 13:00:00"),
                    EndDate = DateTime.Parse("2019-01-07 13:00:00"),
                    MaxAttendees = 10
                };
                var testTrainingProgramJSON = JsonConvert.SerializeObject(tProgram);

                var response = await client.PostAsync(

                    "/api/TrainingProgram",
                    new StringContent(testTrainingProgramJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Jump Java and Whales!", newTrainingProgram.Name);
                Assert.Equal(10, newTrainingProgram.MaxAttendees);



                //Checking Delete Part
                var deleteResponse = await client.DeleteAsync($"/api/TrainingProgram/{newTrainingProgram.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Update_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Checking Update

                TrainingProgram UpdateToNewTrainingProgram = new TrainingProgram
                {
                    Name = "Coding with Emojies",
                    StartDate = DateTime.Parse("2020-01-01 13:00:00"),
                    EndDate = DateTime.Parse("2019-01-07 13:00:00"),
                    MaxAttendees = 100
                };
                var newTrainingJSON = JsonConvert.SerializeObject(UpdateToNewTrainingProgram);


                var response = await client.PutAsync(
                            $"api/TrainingProgram/1",
                            new StringContent(newTrainingJSON, Encoding.UTF8, "application/json")
                        );
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                //  GET single trainingProgram 

                var getNewTrainingProgram = await client.GetAsync($"/api/TrainingProgram/1");
                getNewTrainingProgram.EnsureSuccessStatusCode();

                string getNewTrainingProgramBody = await getNewTrainingProgram.Content.ReadAsStringAsync();
                TrainingProgram newTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getNewTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getNewTrainingProgram.StatusCode);
                Assert.Equal("Coding with Emojies", newTrainingProgram.Name);
            }
        }

        [Fact]
        public async Task Test_Non_Existing_Get_Update_And_Delete_TrainingProgram()
        {
            using (var trainingProgram = new APIClientProvider().Client)
            {

                var response = await trainingProgram.GetAsync("/api/TrainingProgram/99999");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);



                //Checking Update
                TrainingProgram UpdateToNewTrainingProgram = new TrainingProgram
                {
                    Name = "Coding with Emojies",
                    StartDate = DateTime.Parse("2020-01-01 13:00:00"),
                    EndDate = DateTime.Parse("2019-01-07 13:00:00"),
                    MaxAttendees = 100
                };
                var newTrainingProgramJSON = JsonConvert.SerializeObject(UpdateToNewTrainingProgram);


                response = await trainingProgram.PutAsync(
                    $"api/TrainingProgram/99999",
                    new StringContent(newTrainingProgramJSON, Encoding.UTF8, "application/json")
                );
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);


                //Checking Delete Part
                var deleteResponse = await trainingProgram.DeleteAsync($"/api/TrainingProgram/99999");
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

    }
}
