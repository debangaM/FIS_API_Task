using FIS_API_Task.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FIS_API_Task.StepDefinitions
{
    [Binding]
    public class CoinbaseStepDefinitions
    {
        private WebAPIDriver _webDriver;
        private HttpResponseMessage _response;
        private readonly ScenarioContext _scenarioContext;
        public TestContext TestContext { get; set; }

        public CoinbaseStepDefinitions(TestContext testContext, ScenarioContext scenarioContext)
        {
            TestContext = testContext;
            _webDriver = new WebAPIDriver(TestContext, scenarioContext);
            _scenarioContext = scenarioContext;
        }

        [When(@"I perform a Get request to '([^']*)'")]
        public async Task WhenIPerformAGetRequestTo(string url)
        {
            Console.WriteLine("Endpoint: " + url);
            await _webDriver.PerformGetRequest(url);
        }

        [Then(@"the response status code should be '([^']*)'")]
        public void ThenTheResponseStatusCodeShouldBe(string expectedStatus)
        {
            var expectedStatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), expectedStatus);
            _webDriver.ValidateStatusCode(expectedStatusCode);
        }

        [Then(@"the response body includes '([^']*)'")]
        public void ThenTheResponseBodyIncludes(string expectedContent)
        {
            _webDriver.ValidateResponseBodyContains(expectedContent);
        }
        [Then(@"the property '([^']*)' should have '([^']*)' items")]
        public void ThenThePropertyShouldHaveItems(string expectedPropertyName, int expectedCount)
        {
            _webDriver.ValidateItemCount(expectedPropertyName, expectedCount);
        }

        [Then(@"the property '([^']*)' should contain the following child items")]
        public void ThenThePropertyShouldContainTheFollowingChildItems(string expectedProperty, Table table)
        {
            var currencies = table.Rows.Select(row => row[0]).ToArray();
            _webDriver.ValidateChildItems(expectedProperty, currencies);
        }


        [Then(@"the jsonpath '([^']*)' should have value '([^']*)'")]
        public void ThenTheJsonpathShouldHaveValue(string expectedJsonPath, string expectedValue)
        {
            _webDriver.ValidatePropertyValue(expectedJsonPath, expectedValue);
        }


    }
}
