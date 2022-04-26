using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
    /// <summary>
    /// Xunit.Extensions.Ordering test framework, but this will parallelize all tests, even those in the same class.
    /// </summary>
    public class ParallelByClassTestFramework : XunitTestFramework
    {
        public ParallelByClassTestFramework(IMessageSink messageSink)
            : base(messageSink) { }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink, runTestsInClassParallel: true);
        }
    }
}
