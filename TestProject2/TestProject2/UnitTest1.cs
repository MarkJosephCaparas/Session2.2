using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Net.Http;
using System.Text;

namespace TestProject2
{
    [TestClass]
    public class UnitTest1
    {
      
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<ModelForPet> cleanUpList = new List<ModelForPet>();

        [TestInitialize]
        public void TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanUp()
        {
            foreach (var data in cleanUpList)
            {
                var request = new RestRequest(GetURI($"{PetEndpoint}/{data.id}"));
               // await restClient.DeleteAsync(request);
            }
        }

        [TestMethod]
        public async Task PostPet()
        {
            // Create Json Object
            ModelForPet petData = new ModelForPet()
            {
                id = 1234,
                category = new Category()
                {
                    id = 5678,
                    name = "PetCategory"
                },


                name = "Blacky",
                photoUrls = new string[]
                {
                    "Running", "Sleeping"
                },

                tags = new Tag[]
                {
                    new Tag()
                    {
                     id = 0,
                     name = "PetTag"

                    }

                },
                status = "available"
            };

           
            //create Pet
            var createRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(petData);
            await restClient.ExecutePostAsync(createRequest);
           
            //get created Pet
            var getPeRequest = new RestRequest(GetURI($"{PetEndpoint}/{petData.id}"));
            var respone = await restClient.ExecuteAsync<ModelForPet>(getPeRequest);

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, respone.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(petData.name, respone.Data.name, "Pet Name did not match.");
            Assert.AreEqual(petData.category.name, respone.Data.category.name, "Category did not match.");
            for  (int i = 0; i < respone.Data.photoUrls.Count(); i++) {
                Assert.AreEqual(petData.photoUrls[i], respone.Data.photoUrls[i], "PhotoURLs did not match.");
            
            }

            for (int x = 0; x < respone.Data.tags.Count(); x++)
            {
                Assert.AreEqual(petData.tags[x].name, respone.Data.tags[x].name, "Tags did not match.");

            }
            
            
            Assert.AreEqual(petData.status, respone.Data.status, "Status did not match.");
            #endregion

            #region cleanup data

            // Add data to cleanup list
            cleanUpList.Add(petData);

            #endregion

           
        }






       

    }

}
