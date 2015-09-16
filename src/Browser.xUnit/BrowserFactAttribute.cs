using Xunit;
using Xunit.Sdk;

namespace Browser.xUnit
{
    [XunitTestCaseDiscoverer("Browser.xUnit.Sdk.Framework.BrowserFactDiscoverer", "Browser.xUnit")]
    public class BrowserFactAttribute : FactAttribute
    {
    }
}
