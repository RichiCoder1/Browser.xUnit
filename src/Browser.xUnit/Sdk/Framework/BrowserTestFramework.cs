using System.Reflection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk
{
    public class BrowserTestFramework : XunitTestFramework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.XunitTestFramework"/> class.
        /// </summary>
        /// <param name="messageSink">The message sink used to send diagnostic messages</param>
        public BrowserTestFramework(IMessageSink messageSink)
            : base(messageSink)
        {
        }

        /// <inheritdoc/>
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new BrowserTestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}
