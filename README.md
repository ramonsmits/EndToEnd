# EndToEnd

This GitHub repository contains several Visual Studio solutions for end-to-end testing of NServiceBus.

## Goal
Contain automated and manual tests that are run to verify correctness, performance and/or stability of Particular Platform.
The tests are only subset of all verification steps that are done on various steps of platform development process.

## Performance tests
With the performance tests it's possible to measure the performance of features within NServiceBus.

More detailed information on performance tests can be found in the [documentation](docs/performance-index.md).

## Transport compatibility tests
The [Visual Studio solution](EndToEnd/src/TransportCompatibilityTests/TransportCompatibilityTests.sln) provides a framework and tests to verify sending messages over different transport versions. Up until now it's only possible to send messages over the same transport type, so MSMQ to MSMQ.

## Persistence compatibility tests

The [Persistence Compatibility Tests](https://github.com/Particular/EndToEnd/tree/master/src/PersistenceCompatibilityTests) verify that information can be saved and read by various versions of the available [NServiceBus persistences](http://docs.particular.net/nservicebus/persistence/). Those properties are verified on the level of the interfaces.

The tests automatically retrieve from NuGet feeds all minor versions of persistence within a given range. The same test cases are verified for all those versions.

Tested versions:
- NHibernate - versions 4.5 - 7.x

Test cases:
- Saga persistence - testing `ISagaPersistance`
  - Saga data persisted in version X, can be read correctly in version Y, also for complex saga data structures (nested properties, collections)
  - Saga data persisted and updated in version X, can be read correctly in version Y, also for complex saga data structures (nested properties, collections)
  - The data of the saga completed in version X, cannot be accessed in version Y

## Serializer compatibility tests
More info soon...

## Wire compatibility tests
More info soon...
