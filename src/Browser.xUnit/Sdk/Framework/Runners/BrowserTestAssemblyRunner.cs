using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk
{
    public class BrowserTestAssemblyRunner : XunitTestAssemblyRunner
    {
        protected IReadOnlyList<IBeforeAfterAssemblyTests> BeforeAfterHandlers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.XunitTestAssemblyRunner"/> class.
        /// </summary>
        /// <param name="testAssembly">The assembly that contains the tests to be run.</param><param name="testCases">The test cases to be run.</param><param name="diagnosticMessageSink">The message sink to report diagnostic messages to.</param><param name="executionMessageSink">The message sink to report run status to.</param><param name="executionOptions">The user's requested execution options.</param>
        public BrowserTestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
            : base(testAssembly, testCases.Cast<BrowserTestCase>().ToList(), diagnosticMessageSink, executionMessageSink, executionOptions)
        {
            BeforeAfterHandlers =
                testAssembly.Assembly.GetCustomAttributes(typeof(BeforeAfterAssemblyTestsAttribute))
                    .Select(attributeInfo => attributeInfo.GetNamedArgument<Type>("HandlerType"))
                    .Select(handlerType => ExtensibilityPointFactory.Get<IBeforeAfterAssemblyTests>(diagnosticMessageSink, handlerType))
                    .ToList();
        }
        
        /// <inheritdoc/>
        protected override Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
            => new BrowserTestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource).RunAsync();


        /// <inheritdoc/>
        protected override async Task AfterTestAssemblyStartingAsync()
        {
            await base.AfterTestAssemblyStartingAsync();
            foreach (var handler in BeforeAfterHandlers)
            {
                await handler.BeforeTestAssemblyStartedAsync();
            }
        }

        /// <inheritdoc/>
        protected override async Task BeforeTestAssemblyFinishedAsync()
        {
            await base.BeforeTestAssemblyFinishedAsync();
            foreach (var handler in BeforeAfterHandlers)
            {
                await handler.AfterTestAssemblyFinishingAsync();
            }
        }
    }
}
