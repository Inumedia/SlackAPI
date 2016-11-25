[![Build status](https://ci.appveyor.com/api/projects/status/5n9e7sruxpo0mw79/branch/master?svg=true)](https://ci.appveyor.com/project/Inumedia/slackapi/branch/master)

# SlackAPI

This is a third party implementation of Slack's API written in C#. This supports their WebAPI aswell as their Real Time Messaging API.

# Examples

Some examples can be found on the Wiki: https://github.com/Inumedia/SlackAPI/wiki/Examples

# Issues and Bugs

Please log an issue if you find any bugs or think something isn't correct.

# Getting in touch

I have a Slack setup for personal projects with a few friends, this includes this Github and a few others as public channels. If you want access, shoot me a quick email inumedia@inumedia.net.

# Committer access

Want committer access? Feel like I'm too lazy to keep up with Slack's ever changing API? Want a bug fixed but don't want to log an issue for it?

Create some pull requests, give me a reason to give you access.

# Howto build the solution
###### (aka where is the config.json file?)
The project **SlackAPI.Tests** requires a valid `config.json` file for tests. You have two options to build the solution:
- Unload SlackAPI.Tests project and you're able to build SlackAPI solution.
- Create your own config.json file to be able to run tests and validate your changes.
  - Copy/paste `config.default.json` to `config.json`
  - Update `config.json` file with your settings
    - *userAuthToken* : Visit https://api.slack.com/docs/oauth-test-tokens to generate a token for your user
    - *botAuthToken* : Visit https://my.slack.com/services/new/bot to create a bot for your Slack team and retrieve associated token
    - *testChannel* : A channel ID (user associated to *userAuthToken* must be member of the channel)
    - *directMessageUser* : A Slack member username
    - *clientId*/*clientSecret*/*authCode* : Not used

# Creating NuGet package

Example:

```nuget pack SlackAPI.csproj -version 1.2.3```
