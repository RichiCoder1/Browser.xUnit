using System;

using Xunit.Sdk;

namespace Browser.xUnit
{
    [TestFrameworkDiscoverer("Browser.xUnit.Sdk.BrowserTestFrameworkTypeDiscoverer", "Browser.xUnit")]
    [AttributeUsage(AttributeTargets.Assembly)]

    public class BrowserTestFrameworkAttribute : Attribute, ITestFrameworkAttribute
    {
    }
}
