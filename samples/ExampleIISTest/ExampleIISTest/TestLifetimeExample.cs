using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

using Browser.xUnit;

using ExampleTest;
using Microsoft.Framework.Configuration;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: BrowserTestFramework]
[assembly: BeforeAfterAssemblyTests(typeof(TestLifetimeExample))]

namespace ExampleTest
{
    public class TestLifetimeExample : IBeforeAfterAssemblyTests
    {
        private readonly IMessageSink _messageSink;
        private Process _webProcess;
        public static readonly IConfigurationRoot Configuration;

        static TestLifetimeExample()
        {
            Configuration = new ConfigurationBuilder(Assembly.GetExecutingAssembly().Location)
                .AddJsonFile("config.json")
                .Build();
        }

        public TestLifetimeExample(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        /// <summary>
        /// Executes before execution of an assemblies tests.
        /// </summary>
        public Task BeforeTestAssemblyStartedAsync()
        {
            var args = GetProcessArguments();
            _messageSink.OnMessage(new DiagnosticMessage($"Command: {args.Item1} {args.Item2}"));
            _webProcess = Process.Start(args.Item1, args.Item2);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Executes after execution of an assemblies tests.
        /// </summary>
        public Task AfterTestAssemblyFinishingAsync()
        {
            _messageSink.OnMessage(new DiagnosticMessage($"Cleaning up Kestrel"));
            _webProcess.Kill();
            _webProcess.Dispose();
            return Task.FromResult(true);
        }

        public Tuple<string, string> GetProcessArguments()
        {
            var command = Configuration["Tests:ExampleSite:Command"];
            var process = command.Substring(0, command.IndexOf(" ", StringComparison.Ordinal));
            var arguments = command.Substring(command.IndexOf(" ", StringComparison.Ordinal));
            return Tuple.Create(process, arguments);
        } 
    }
}
