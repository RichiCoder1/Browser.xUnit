using System;
using System.Net.Http;
using System.Threading.Tasks;

using Browser.xUnit;

using Xunit;
using Xunit.Abstractions;

namespace ExampleTest
{
    public class ExampleTest : INeedToKnowTestFailure
    {
        private string Url => TestLifetimeExample.Configuration["Tests:ExampleSite:Url"];

        private HttpClient Client => new HttpClient {BaseAddress = new Uri(Url)};

        [BrowserFact]
        public async Task PassingTest()
        {
            var result = await Client.GetAsync("index.html");
            result.EnsureSuccessStatusCode();
        }

        [BrowserFact]
        public async Task FailingTest()
        {
            var result = await Client.GetAsync("index.json");
            result.EnsureSuccessStatusCode();
        }

        #region Implementation of INeedToKnowTestFailure

        public Task HandleFailureAsync(ITest test, Exception exception)
        {
            return Task.FromResult(true);
        }

        #endregion
    }
}
