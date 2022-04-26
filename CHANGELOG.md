# Changelog

All notable changes to **Dangl.Xunit.Extensions.Ordering** are documented here.

## v2.1.0:
- Added a new test framework `Xunit.Extensions.Ordering.ParallelByClassTestFramework`. This has the same behaviour as the original one, but parallelizes all tests inside single classes as well. It respects explicit `Collection` attributes, but otherwise places each single test in a collection of it's own for maximized parallelization

## v2.0.0
- Initial release of the fork
- The library now targets `netstandard2.0`
- The package now has a strong name and is signed

