[![NuGet](https://img.shields.io/nuget/v/SlackWorkAPI.svg)](https://www.nuget.org/packages/SlackWorkAPI/)

# SlackWorkAPI

This is a third party implementation of Slack's API written in C#. This supports their WebAPI as well as their Real Time Messaging API.

# Examples

Some examples can be found on the Wiki: https://github.com/Inumedia/SlackAPI/wiki/Examples

# NuGet package
SlackAPI NuGet package is build with following platforms support:
- .NET Framework 4.5
- .NET Standard 1.3 (UWP support).
  - The version cannot detect SlackSocketRouting attributes in loaded assemblies (used to extend SlackAPI to handle custom messages).
- .NET Standard 1.6
- .NET Standard 2.0

[(.NET implementation compatibility table)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support)
