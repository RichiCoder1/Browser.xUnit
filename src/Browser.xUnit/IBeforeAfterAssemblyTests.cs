using System.Reflection;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace Browser.xUnit
{
    public interface IBeforeAfterAssemblyTests
    {
        /// <summary>
        /// Executes before execution of an assemblies tests.
        /// </summary>
        Task BeforeTestAssemblyStartedAsync();

        /// <summary>
        /// Executes after execution of an assemblies tests.
        /// </summary>
        Task AfterTestAssemblyFinishingAsync();
    }
}
