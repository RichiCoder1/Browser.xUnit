using System;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace Browser.xUnit
{
    public interface INeedToKnowTestFailure
    {
        Task HandleFailureAsync(ITest test, Exception exception);
    }
}
