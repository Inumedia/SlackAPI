using Xunit;

namespace SlackAPI.Tests.Configuration
{
  [CollectionDefinition("Integration tests")]
  public class IntegrationCollection : ICollectionFixture<IntegrationFixture>
  {
  }
}
