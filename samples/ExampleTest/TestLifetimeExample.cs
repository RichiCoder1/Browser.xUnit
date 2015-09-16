using System;
using System.Threading.Tasks;

using Browser.xUnit;

using ExampleTest;

using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: BrowserTestFramework]
[assembly: BeforeAfterAssemblyTests(typeof(TestLifetimeExample))]

namespace ExampleTest
{
    public class TestLifetimeExample : IBeforeAfterAssemblyTests
    {
        private readonly IMessageSink _messageSink;

        public TestLifetimeExample(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        /// <summary>
        /// Executes before execution of an assemblies tests.
        /// </summary>
        public Task BeforeTestAssemblyStartedAsync()
        {
            _messageSink.OnMessage(new DiagnosticMessage("BeforeTestAssemblyStartedAsync"));
            return Task.FromResult(true);
        }

        /// <summary>
        /// Executes after execution of an assemblies tests.
        /// </summary>
        public Task AfterTestAssemblyFinishingAsync()
        {
            _messageSink.OnMessage(new DiagnosticMessage("AfterTestAssemblyFinishingAsync"));
            return Task.FromResult(true);
        }
    }
}
