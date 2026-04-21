using Heroes.Server.Tests.Infrastructure;

namespace Heroes.Server.Tests;

/// <summary>
/// Shares a single <see cref="HeroesWebApplicationFactory"/> across all test classes in the
/// "Integration" collection so the embedded Orleans silo is started only once per test run.
/// </summary>
[CollectionDefinition("Integration")]
public sealed class IntegrationCollection : ICollectionFixture<HeroesWebApplicationFactory>;
