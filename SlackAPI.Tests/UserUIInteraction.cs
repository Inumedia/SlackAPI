using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SlackAPI.Tests.Configuration;
using SlackAPI.Tests.Helpers;
using Xunit;

namespace SlackAPI.Tests
{
    [Collection("Integration tests")]
    public class UserUIInteraction
    {
        private readonly IntegrationFixture fixture;

        public UserUIInteraction(IntegrationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestGetAccessToken()
        {
            var clientId = this.fixture.Config.ClientId;
            var clientSecret = this.fixture.Config.ClientSecret;
            var redirectUrl = this.fixture.Config.RedirectUrl;
            var authUsername = this.fixture.Config.AuthUsername;
            var authPassword = this.fixture.Config.AuthPassword;
            var authWorkspace = this.fixture.Config.AuthWorkspace;

            using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
            {
                driver.Navigate().GoToUrl($"https://{authWorkspace}.slack.com");

                // Wait a bit to ensure we can properly fill username and password fields
                Thread.Sleep(1000);
                driver.FindElement(By.Id("email")).SendKeys(authUsername);
                driver.FindElement(By.Id("password")).SendKeys(authPassword);
                driver.FindElement(By.Id("signin_btn")).Click();

                var slackClientHelpers = new SlackClientHelpers();
                var uri = slackClientHelpers.GetAuthorizeUri(clientId, SlackScope.Identify);
                driver.Navigate().GoToUrl(uri);
                driver.FindElement(By.Id("oauth_authorizify")).Click();

                var code = Regex.Match(driver.Url, "code=(?<code>[^&]+)&state").Groups["code"].Value;

                var accessTokenResponse = GetAccessToken(slackClientHelpers, clientId, clientSecret, redirectUrl, code);
                Assert.True(accessTokenResponse.ok);
                Assert.Equal("identify", accessTokenResponse.scope);
            }
        }

        private AccessTokenResponse GetAccessToken(SlackClientHelpers slackClientHelpers, string clientId, string clientSecret, string redirectUri, string authCode)
        {
            AccessTokenResponse accessTokenResponse = null;

            using (var sync = new InSync(nameof(slackClientHelpers.GetAccessToken)))
            {
                slackClientHelpers.GetAccessToken(response =>
                {
                    accessTokenResponse = response;
                    sync.Proceed();

                }, clientId, clientSecret, redirectUri, authCode);
            }

            return accessTokenResponse;
        }
    }
}
