![Rocket.Chat.Net](/docs/images/icon.png)

# Rocket.Chat.Net

|                     | Pre-release                                 | Stable Release |
| ------------------- | ------------------------------------------- | ---            |
| Rocket.Chat.Net     | [![NuGet][Base-Nuget-Pre-Img]][Base-Nuget-Link] | [![NuGet][Base-Nuget-Pre-Img]][Base-Nuget-Link] |
| Rocket.Chat.Net.Bot | [![NuGet][Bot-Nuget-Pre-Img]][Bot-Nuget-Link]   | [![NuGet][Bot-Nuget-Pre-Img]][Bot-Nuget-Link] |

[Base-Nuget-Pre-Img]: https://img.shields.io/nuget/vpre/Rocket.Chat.Net.svg?style=flat-square&maxAge=3600
[Bot-Nuget-Pre-Img]: https://img.shields.io/nuget/vpre/Rocket.Chat.Net.Bot.svg?style=flat-square&maxAge=3600
[Base-Nuget-Img]: https://img.shields.io/nuget/v/Rocket.Chat.Net.svg?style=flat-square&maxAge=3600
[Bot-Nuget-Img]: https://img.shields.io/nuget/v/Rocket.Chat.Net.Bot.svg?style=flat-square&maxAge=3600

[Base-Nuget-Link]: https://www.nuget.org/packages/Rocket.Chat.Net/
[Bot-Nuget-Link]: https://www.nuget.org/packages/Rocket.Chat.Net.Bot/


[![Build status](https://img.shields.io/appveyor/ci/Silvenga/rocket-chat-net.svg?style=flat-square&maxAge=300&label=appveyor)](https://ci.appveyor.com/project/Silvenga/rocket-chat-net) 
[![Build status](https://img.shields.io/travis/Silvenga/Rocket.Chat.Net.svg?style=flat-square&maxAge=300&label=travis)](https://travis-ci.org/Silvenga/Rocket.Chat.Net) 
[![Coverage Status](https://img.shields.io/coveralls/Silvenga/Rocket.Chat.Net.svg?style=flat-square&maxAge=300)](https://coveralls.io/github/Silvenga/Rocket.Chat.Net?branch=master)
[![License](https://img.shields.io/github/license/Silvenga/Rocket.Chat.Net.svg?style=flat-square&maxAge=604800)](https://github.com/Silvenga/Rocket.Chat.Net/blob/master/LICENSE)

A Rocket.Chat real-time, managed, .Net driver, and bot. 

## Usage

```csharp
const string username = "m@silvenga.com";
const string password = "silverlight";
const string rocketServerUrl = "dev0:3000"; // just the host and port
const bool useSsl = false; // Basically use ws or wss.

// Basic logger
ILogger logger = new ConsoleLogger();

// Create the bot - an abstraction of the driver
RocketChatBot bot = new RocketChatBot(rocketServerUrl, useSsl, logger);

// Connect to Rocket.Chat
await bot.ConnectAsync();

// Login
ILoginOption loginOption = new EmailLoginOption
{
    Email = username,
    Password = password
};
await bot.LoginAsync(loginOption);

// Start listening for messages
await bot.SubscribeAsync();

// Add possible responses to be checked in order
// This is not thead safe, FYI 
IBotResponse giphyResponse = new GiphyResponse();
bot.AddResponse(giphyResponse);

// And that's it
// Checkout GiphyResponse in the example project for more info.
```

## Building/Testing

[docs/environment.md](docs/environment.md)

## Version compatibility changelogs

[version-compatibility.md](docs/version-compatibility.md)

## Feature Support/TODO

[docs/todo.md](docs/todo.md)