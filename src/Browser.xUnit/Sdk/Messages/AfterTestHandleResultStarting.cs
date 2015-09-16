using Xunit.Abstractions;
using Xunit.Sdk;

namespace Browser.xUnit.Sdk.Messages
{
    public class AfterTestHandleResultStarting : TestMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Xunit.Sdk.TestMessage"/> class.
        /// </summary>
        public AfterTestHandleResultStarting(ITest test)
            : base(test)
        {
        }
    }
}
