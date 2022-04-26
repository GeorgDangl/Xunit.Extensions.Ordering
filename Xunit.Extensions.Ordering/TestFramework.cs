﻿using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
    /// <summary>
    /// Xunit.Extensions.Ordering test framework.
    /// </summary>
    public class TestFramework : XunitTestFramework
    {
        public TestFramework(IMessageSink messageSink)
            : base(messageSink) { }

        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
        {
            return new TestFrameworkExecutor(assemblyName, SourceInformationProvider, DiagnosticMessageSink, runTestsInClassParallel: false);
        }
    }
}
