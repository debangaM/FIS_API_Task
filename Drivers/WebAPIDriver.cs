using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FIS_API_Task.Drivers
{
    public class WebAPIDriver
    {
        private string baseURL;
        private FlurlClient client;
        private HttpResponseMessage? response;
        private readonly ScenarioContext _scenarioContext;

        public WebAPIDriver(TestContext testContext, ScenarioContext scenarioContext)
        {
            baseURL = testContext.Properties["webAppUrl"]?.ToString();
            Console.WriteLine("Base url: " + baseURL);
            if (string.IsNullOrEmpty(baseURL))
            {
                throw new InvalidOperationException("webAppUrl is not configured in runsettings.");
            }
            client = new FlurlClient(baseURL);
            _scenarioContext = scenarioContext;
        }

        public async Task PerformGetRequest(string url, object queryParams = null)
        {
            response = await client
                .Request(url)
                .SetQueryParams(queryParams)
                .GetAsync();
            await LogAPICallDetails();
            _scenarioContext["HttpResponse"] = response;
        }

        public void ValidateStatusCode(HttpStatusCode expectedStatusCode)
        {
            if (response.StatusCode != expectedStatusCode)
            {
                throw new Exception($"Expected status code {expectedStatusCode}, but got {response.StatusCode}");
            }
        }

        public void ValidateResponseBodyContains(string expectedContent)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            if (!responseBody.Contains(expectedContent))
            {
                throw new Exception($"Expected content not found in response: {expectedContent}");
            }
        }

        public async Task LogAPICallDetails()
        {
            Console.WriteLine(response.ToString());
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Content:");
            Console.WriteLine(responseContent);
        }

        public void ValidateItemCount(string expectedPropertyName, int expectedCount)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(responseBody);

            var propertyName = json[expectedPropertyName];
            if (propertyName == null)
            {
                throw new Exception("Property not found in the response.");
            }

            int actualCount = propertyName.Children().Count();
            if (actualCount != expectedCount)
            {
                throw new Exception($"Expected {expectedCount} property items, but found {actualCount}.");
            }
        }
        public void ValidateChildItems(string expectedProperty, params string[] expectedItems)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(responseBody);

            var propertyName = json[expectedProperty];
            if (propertyName == null)
            {
                throw new Exception("Property not found in the response.");
            }

            foreach (var item in expectedItems)
            {
                Console.WriteLine($"Checking for property: {item}");
                if (propertyName[item] == null)
                {
                    throw new Exception($"Property item '{item}' not found in the response.");
                }
            }
        }

        public void ValidatePropertyValue(string expectedJsonPath, string expectedValue)
        {
            string responseBody = response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(responseBody);

            var jsonPath = json.SelectToken(expectedJsonPath);
            if (jsonPath == null)
            {
                throw new Exception($"{jsonPath} not found.");
            }

            var description = jsonPath?.ToString();
            if (description == null)
            {
                throw new Exception($"{jsonPath} is null.");
            }

            if (!description.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Expected value to be '{expectedValue}', but found '{description}'.");
            }
        }
    }
}
