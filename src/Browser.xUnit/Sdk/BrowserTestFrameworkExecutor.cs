using System.Collections.Generic;
using System.Reflection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk
{
    public class BrowserTestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.XunitTestFrameworkExecutor"/> class.
        /// </summary>
        /// <param name="assemblyName">Name of the test assembly.</param><param name="sourceInformationProvider">The source line number information provider.</param><param name="diagnosticMessageSink">The message sink to report diagnostic messages to.</param>
        public BrowserTestFrameworkExecutor(
            AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider,
            IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
        }

        /// <inheritdoc/>
        protected override async void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            using (var assemblyRunner = CreateTestAssemblyRunner(testCases, executionMessageSink, executionOptions))
                await assemblyRunner.RunAsync();
        }

        protected virtual TestAssemblyRunner<IXunitTestCase> CreateTestAssemblyRunner(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
            =>
                new BrowserTestAssemblyRunner(
                    TestAssembly,
                    testCases,
                    DiagnosticMessageSink,
                    executionMessageSink,
                    executionOptions);
    }
}
