using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk
{
    public class BrowserTestCollectionRunner : XunitTestCollectionRunner
    {
        protected IMessageSink DiagnosticMessageSink { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.XunitTestCollectionRunner"/> class.
        /// </summary>
        /// <param name="testCollection">The test collection that contains the tests to be run.</param><param name="testCases">The test cases to be run.</param><param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param><param name="messageBus">The message bus to report run status to.</param><param name="testCaseOrderer">The test case orderer that will be used to decide how to order the test.</param><param name="aggregator">The exception aggregator used to run code and collect exceptions.</param><param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public BrowserTestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
            DiagnosticMessageSink = diagnosticMessageSink;
        }

        /// <inheritdoc/>
        protected override Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
            => new BrowserTestClassRunner(testClass, @class, testCases, DiagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings).RunAsync();

    }
}
