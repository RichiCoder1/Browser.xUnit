using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk
{
    public class BrowserTestInvoker : XunitTestInvoker
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.XunitTestInvoker"/> class.
        /// </summary>
        /// <param name="test">The test that this invocation belongs to.</param><param name="messageBus">The message bus to report run status to.</param><param name="testClass">The test class that the test method belongs to.</param><param name="constructorArguments">The arguments to be passed to the test class constructor.</param><param name="testMethod">The test method that will be invoked.</param><param name="testMethodArguments">The arguments to be passed to the test method.</param><param name="beforeAfterAttributes">The list of <see cref="T:Xunit.Sdk.BeforeAfterTestAttribute"/>s for this test invocation.</param><param name="aggregator">The exception aggregator used to run code and collect exceptions.</param><param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public BrowserTestInvoker(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments, MethodInfo testMethod, object[] testMethodArguments, IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
        }

        /// <inheritdoc/>
        protected override async Task<decimal> InvokeTestMethodAsync(object testClassInstance)
        {
            var oldSyncContext = SynchronizationContext.Current;

            try
            {
                var asyncSyncContext = new AsyncTestSyncContext(oldSyncContext);
                SetSynchronizationContext(asyncSyncContext);
                
                Exception testException = null;

                await Aggregator.RunAsync(
                    () => Timer.AggregateAsync(
                        async () =>
                        {
                            var parameterCount = TestMethod.GetParameters().Length;
                            var valueCount = TestMethodArguments == null ? 0 : TestMethodArguments.Length;
                            if (parameterCount != valueCount)
                            {
                                Aggregator.Add(
                                    new InvalidOperationException(
                                        $"The test method expected {parameterCount} parameter value{(parameterCount == 1 ? "" : "s")}," + 
                                        $"but {valueCount} parameter value{(valueCount == 1 ? "" : "s")} {(valueCount == 1 ? "was" : "were")} provided.")
                                );
                            }
                            else
                            {
                                try {
                                    var result = TestMethod.Invoke(testClassInstance, TestMethodArguments);
                                    var task = result as Task;
                                    if (task != null)
                                        await task;
                                    else
                                    {
                                        var ex = await asyncSyncContext.WaitForCompletionAsync();
                                        if (ex != null)
                                        {
                                            testException = ex;
                                            Aggregator.Add(ex);
                                        }
                                    }
                                } catch (Exception ex) {
                                    testException = ex;
                                    throw;
                                }
                            }
                        }
                    )
                );
                
                if (testException != null)
                {
                    var handleTestFailure = testClassInstance as INeedToKnowTestFailure;
                    if (handleTestFailure != null)
                    {
                        await
                            Aggregator.RunAsync(
                                    () => handleTestFailure.HandleFailureAsync(Test, testException));
                    }
                }
            }
            finally
            {
                SetSynchronizationContext(oldSyncContext);
            }

            return Timer.Total;
        }

        [SecuritySafeCritical]
        static void SetSynchronizationContext(SynchronizationContext context)
        {
            SynchronizationContext.SetSynchronizationContext(context);
        }
    }
}
