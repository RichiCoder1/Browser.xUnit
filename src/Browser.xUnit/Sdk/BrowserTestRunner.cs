using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk
{
    public class BrowserTestRunner : XunitTestRunner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.XunitTestRunner"/> class.
        /// </summary>
        /// <param name="test">The test that this invocation belongs to.</param><param name="messageBus">The message bus to report run status to.</param><param name="testClass">The test class that the test method belongs to.</param><param name="constructorArguments">The arguments to be passed to the test class constructor.</param><param name="testMethod">The test method that will be invoked.</param><param name="testMethodArguments">The arguments to be passed to the test method.</param><param name="skipReason">The skip reason, if the test is to be skipped.</param><param name="beforeAfterAttributes">The list of <see cref="T:Xunit.Sdk.BeforeAfterTestAttribute"/>s for this test.</param><param name="aggregator">The exception aggregator used to run code and collect exceptions.</param><param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public BrowserTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, string skipReason, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        /// <inheritdoc/>
        protected override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
           => new BrowserTestInvoker(Test, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, BeforeAfterAttributes, aggregator, CancellationTokenSource).RunAsync();
    }
}
