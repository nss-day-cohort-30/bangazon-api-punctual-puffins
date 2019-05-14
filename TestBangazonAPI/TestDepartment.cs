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
    public class TestDepartment
    {

        [Fact]
        public async Task Test_Get_Single_Department()
        {

            using (var department = new APIClientProvider().Client)
            {
                var response = await department.GetAsync("/api/Department/2");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var departmentResponse = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("IT", departmentResponse.Name);
                Assert.NotNull(departmentResponse);
            }
        }
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Department");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }
        // get all department with employees
        [Fact]
        public async Task Test_Get_All_Departments_with_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Department?_include=employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }

        //with budget with gt
        [Fact]
        public async Task Test_Get_All_Departments_with_Filter_Budget()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Department?_filter=budget&_gt=30000");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }
        //with budget with gt and with employees
        [Fact]
        public async Task Test_Get_All_Departments_with_Filter_Budget_and_employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/Department?_filter=budget&_gt=30000&_include=employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }



        [Fact]
        public async Task Test_Create_And_Delete_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                Department dept = new Department
                {
                    Name = "TestDept",
                    Budget = 200
                };
              var testDeptJSON = JsonConvert.SerializeObject(dept);

                var response = await client.PostAsync(

                    "/api/Department",
                    new StringContent(testDeptJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newDept = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("TestDept", newDept.Name);
                Assert.Equal(200, newDept.Budget);

                

                //Checking Delete Part
                var deleteResponse = await client.DeleteAsync($"/api/Department/{newDept.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Update_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Checking Update
                
                Department UpdateToNewDept = new Department
                {
                    Name = "Customer Service",
                    Budget = 500
                };
                var newDeptJSON = JsonConvert.SerializeObject(UpdateToNewDept);


                var response = await client.PutAsync(
                            $"api/Department/1",
                            new StringContent(newDeptJSON, Encoding.UTF8, "application/json")
                        );
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                //  GET single department 

                var getNewDept = await client.GetAsync($"/api/Department/1");
                getNewDept.EnsureSuccessStatusCode();

                string getNewDeptBody = await getNewDept.Content.ReadAsStringAsync();
                Department newDept = JsonConvert.DeserializeObject<Department>(getNewDeptBody);

                Assert.Equal(HttpStatusCode.OK, getNewDept.StatusCode);
                Assert.Equal("Customer Service", newDept.Name);
            }
        }

        [Fact]
        public async Task Test_Non_Existing_Get_Update_And_Delete_Department()
        {
            using (var department = new APIClientProvider().Client)
            {
                
                var response = await department.GetAsync("/api/Department/99999");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                   
            
               
                //Checking Update
                Department UpdateToNewDept = new Department
                {
                    Name = "New Dept",
                    Budget = 500
                };
                var newDeptJSON = JsonConvert.SerializeObject(UpdateToNewDept);


                response = await department.PutAsync(
                    $"api/Department/99999",
                    new StringContent(newDeptJSON, Encoding.UTF8, "application/json")
                );
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);


                //Checking Delete Part
                var deleteResponse = await department.DeleteAsync($"/api/Department/99999");
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

    }
}
