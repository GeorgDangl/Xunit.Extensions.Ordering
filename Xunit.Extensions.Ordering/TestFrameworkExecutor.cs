using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
    /// <summary>
    /// Xunit.Extensions.Ordering test framework executor.
    /// </summary>
    public class TestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        private readonly bool _runTestsInClassParallel;

        public TestFrameworkExecutor(
            AssemblyName assemblyName,
            ISourceInformationProvider sourceInformationProvider,
            IMessageSink diagnosticMessageSink,
            bool runTestsInClassParallel)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {
            _runTestsInClassParallel = runTestsInClassParallel;
        }

        protected override async void RunTestCases(
            IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink,
            ITestFrameworkExecutionOptions executionOptions)
        {
            testCases = GetCurrentTestCases(testCases);
            using (var assemblyRunner =
                new TestAssemblyRunner(
                    TestAssembly,
                    testCases,
                    DiagnosticMessageSink,
                    executionMessageSink,
                    executionOptions))
                await assemblyRunner.RunAsync();
        }

        private IEnumerable<IXunitTestCase> GetCurrentTestCases(IEnumerable<IXunitTestCase> originalTestCases)
        {
            if (!_runTestsInClassParallel)
            {
                return originalTestCases;
            }

            return TestCollectionsPerEachSingleTestFactory.GetTestCasesParallelInsideClass(originalTestCases, DiagnosticMessageSink);
        }
    }
}
