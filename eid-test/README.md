# How to run test

Depending on the type of test, different conditions must be met. Tests not located in the 'WithoutReader' or 'WithoutCard'
folder require a compatible reader with eID card to be present during the execution of the tests.

Some tests involve a specific card reader and/or card. Those tests use the `TestContext.READER_NAME` or `TestContext.CARD_NAME` const locate in the
`TestContext.cs` of the test project. Change the values to match your test reader and/or card.
