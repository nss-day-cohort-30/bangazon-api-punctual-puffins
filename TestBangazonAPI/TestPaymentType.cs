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
    public class TestPaymentType
    {

        [Fact]
        public async Task Test_Get_Single_PaymentType()
        {

            using (var paymenttypes = new APIClientProvider().Client)
            {
                var response = await paymenttypes.GetAsync("paymenttype/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypeResponse = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Ryan Nelson", paymenttypeResponse.Name);
                Assert.NotNull(paymenttypeResponse);
            }
        }
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymenttype");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypes.Count > 0);
            }
        }
        // get all department with employees
        [Fact]
        public async Task Test_Get_All_PaymentTypes_with_Names()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymenttype?_include=names");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypes.Count > 0);
            }
        }

        //with budget with gt
        [Fact]
        public async Task Test_Get_All_PaymentTypes_with_Filter_AcctNumber()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymenttype?_filter=AcctNumber&_gt=123123");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypes.Count > 0);
            }
        }
        //with budget with gt and with employees

        [Fact]
        public async Task Test_Get_All_PaymentTypes_with_Filter_CustomerId()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymenttype?_filter=CustomerId&_gt=1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypes.Count > 0);
            }
        }
        //with budget with gt and with employees
        [Fact]
        public async Task Test_Get_All_PaymentTypes_with_Filter_AcctNumber_Names_and_CustomerId()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/paymenttype?_filter=AcctNumber&_gt=123123&_include=Names_CustomerId&_gt=1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypes.Count > 0);
            }
        }



        [Fact]
        public async Task Test_Create_And_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType paymenttype = new PaymentType
                {
                    Name = "Ryan Nelson",
                    AcctNumber = 123123,
                    CustomerId = 1
                };
                var testpaymenttypeJSON = JsonConvert.SerializeObject(paymenttype);

                var response = await client.PostAsync(

                    "/paymenttype",
                    new StringContent(testpaymenttypeJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newpaymenttype = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Ryan Nelson", newpaymenttype.Name);
                Assert.Equal(123123, newpaymenttype.AcctNumber);
                Assert.Equal(1, newpaymenttype.CustomerId);



                //Checking Delete Part
                var deleteResponse = await client.DeleteAsync($"/paymenttype/{newpaymenttype.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Update_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Checking Update

                PaymentType UpdateToNewPaymentType = new PaymentType
                {
                    Name = "Ryan Nelson",
                    AcctNumber = 123123,
                    CustomerId = 1
                };
                var newpaymenttypeJSON = JsonConvert.SerializeObject(UpdateToNewPaymentType);


                var response = await client.PutAsync(
                            $"/paymenttype/1",
                            new StringContent(newpaymenttypeJSON, Encoding.UTF8, "application/json")
                        );
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                //  GET single department 

                var getNewpaymenttype = await client.GetAsync($"/paymenttype/1");
                getNewpaymenttype.EnsureSuccessStatusCode();

                string getNewpaymenttypeBody = await getNewpaymenttype.Content.ReadAsStringAsync();
                PaymentType newpaymenttype = JsonConvert.DeserializeObject<PaymentType>(getNewpaymenttypeBody);

                Assert.Equal(HttpStatusCode.OK, getNewpaymenttype.StatusCode);
                Assert.Equal("Ryan Nelson", newpaymenttype.Name);
            }
        }

        [Fact]
        public async Task Test_Non_Existing_Get_Update_And_Delete_PaymentType()
        {
            using (var paymenttype = new APIClientProvider().Client)
            {

                var response = await paymenttype.GetAsync("/paymenttype/99999");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);



                //Checking Update
                PaymentType UpdateToNewpaymenttype = new PaymentType
                {
                    Name = "Ryan Nelson",
                    AcctNumber = 123123,
                    CustomerId = 1
                };
                var newpaymenttypeJSON = JsonConvert.SerializeObject(UpdateToNewpaymenttype);


                response = await paymenttype.PutAsync(
                    $"paymenttype/99999",
                    new StringContent(newpaymenttypeJSON, Encoding.UTF8, "application/json")
                );
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);


                //Checking Delete Part
                var deleteResponse = await paymenttype.DeleteAsync($"/paymenttype/99999");
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

    }
}