using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.Extensions.Ordering
{
    public static class TestCollectionsPerEachSingleTestFactory
    {
        // Essentially taken from here:
        // https://www.meziantou.net/parallelize-test-cases-execution-in-xunit.htm
        // This class is used to create a test collection for each test method in a test class,
        // with the goal of highly parallelizing the execution of the tests.

        public static IEnumerable<IXunitTestCase> GetTestCasesParallelInsideClass(IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink)
        {
            var result = new List<IXunitTestCase>();
            foreach (var testCase in testCases)
            {
                var oldTestMethod = testCase.TestMethod;
                var oldTestClass = oldTestMethod.TestClass;
                var oldTestCollection = oldTestMethod.TestClass.TestCollection;

                // If the collection is explicitly set, don't try to parallelize test execution
                if (oldTestCollection.CollectionDefinition != null || oldTestClass.Class.GetCustomAttributes(typeof(CollectionAttribute)).Any())
                {
                    result.Add(testCase);
                    continue;
                }

                // Create a new collection with a unique id for the test case.
                var newTestCollection =
                        new TestCollection(
                            oldTestCollection.TestAssembly,
                            oldTestCollection.CollectionDefinition,
                            displayName: $"{oldTestCollection.DisplayName} {oldTestCollection.UniqueID}");
                newTestCollection.UniqueID = Guid.NewGuid();

                // Duplicate the test and assign it to the new collection
                var newTestClass = new TestClass(newTestCollection, oldTestClass.Class);
                var newTestMethod = new TestMethod(newTestClass, oldTestMethod.Method);
                switch (testCase)
                {
                    // Used by Theory having DisableDiscoveryEnumeration or non-serializable data
                    case XunitTheoryTestCase xunitTheoryTestCase:
                        result.Add(new XunitTheoryTestCase(
                            diagnosticMessageSink,
                            GetTestMethodDisplay(xunitTheoryTestCase),
                            GetTestMethodDisplayOptions(xunitTheoryTestCase),
                            newTestMethod));
                        break;

                    // Used by all other tests
                    case XunitTestCase xunitTestCase:
                        result.Add(new XunitTestCase(
                            diagnosticMessageSink,
                            GetTestMethodDisplay(xunitTestCase),
                            GetTestMethodDisplayOptions(xunitTestCase),
                            newTestMethod,
                            xunitTestCase.TestMethodArguments));
                        break;

                    // TODO If you use custom attribute, you may need to add cases here

                    default:
                        throw new ArgumentOutOfRangeException("Test case " + testCase.GetType() + " not supported");
                }
            }

            return result;
        }

        private static TestMethodDisplay GetTestMethodDisplay(TestMethodTestCase testCase)
        {
            return (TestMethodDisplay)typeof(TestMethodTestCase)
                .GetProperty("DefaultMethodDisplay", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(testCase)!;
        }

        private static TestMethodDisplayOptions GetTestMethodDisplayOptions(TestMethodTestCase testCase)
        {
            return (TestMethodDisplayOptions)typeof(TestMethodTestCase)
                .GetProperty("DefaultMethodDisplayOptions", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(testCase)!;
        }
    }
}
